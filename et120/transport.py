"""Serial transport: finding the instrument and talking to it.

Platform-independent -- port discovery goes through pyserial, so the same code
works with COM ports on Windows and /dev/tty* on Linux and macOS.
"""

import sys
import time

from .decode import amplitude_agreement, decode_packet, validate
from .protocol import (
    CMD_DEEP, CMD_LOAD_STORED, CMD_PING, CMD_SCREEN,
    PKT_DEEP, PKT_HELLO, PKT_SCREEN, PKT_STORED,
    build_command, iter_packets,
)

USB_VID = 0x28E9        # GD32/Geehy CDC bridge used by these instruments

SCREEN_PACKET_LEN = 1358    # type byte + 600*2 samples + 156 metadata


def _import_serial():
    try:
        import serial
        return serial
    except ImportError:
        sys.exit("pyserial is required:  python -m pip install pyserial")


def list_ports():
    _import_serial()
    from serial.tools import list_ports as lp
    return list(lp.comports())


def describe_ports(ports):
    return "\n".join("  %-14s %s" % (p.device, p.description or "") for p in ports)


def resolve_port(explicit=None):
    """Use the port given, else find the instrument by its USB vendor ID."""
    if explicit:
        return explicit
    ports = list_ports()
    matches = [p for p in ports if p.vid == USB_VID]
    if len(matches) == 1:
        return matches[0].device
    if not matches:
        sys.exit("could not find the instrument automatically.\n"
                 "Give the port explicitly with -p, e.g. -p COM5 or -p /dev/ttyACM0.\n"
                 "Ports seen:\n" + (describe_ports(ports) or "  (none)"))
    sys.exit("several candidate devices found; pick one with -p:\n"
             + describe_ports(matches))


class Scope(object):
    """A connection to the instrument."""

    def __init__(self, port, baud=115200, verbose=False, min_interval=None):
        serial = _import_serial()
        self.port_name = port
        self.min_interval = (self.MIN_COMMAND_INTERVAL if min_interval is None
                             else min_interval)
        self._last_send = 0.0
        # Set this before doing anything when the instrument may be holding a
        # Single-shot acquisition. Command 3 asks it to go live, and issuing
        # that from a held Single-shot hangs the firmware hard enough to need
        # a USB disconnect and power cycle.
        self.avoid_live = False
        try:
            # Match the vendor application's port configuration exactly. It
            # uses .NET's SerialPort and sets only BaudRate, Parity, StopBits,
            # DataBits and ReadBufferSize, leaving everything else at the .NET
            # defaults -- which differ from pyserial's in ways that matter:
            #
            #   property        vendor (.NET)     pyserial default
            #   DtrEnable       false             dtr = True
            #   RtsEnable       false             rts = True
            #   Handshake       None              (equivalent)
            #   ReadBufferSize  100000            4096
            #
            # DTR and RTS are the important pair. pyserial raises both on open,
            # and a USB-CDC device whose firmware reacts to those lines can
            # hang or reset when they toggle -- the mechanism behind Arduino
            # auto-reset. This instrument has hung on exchanges as small as a
            # ping plus two live-mode requests, which implicates opening the
            # port rather than anything sent over it. Setting the states before
            # open() makes pyserial apply them while configuring the port, so
            # the lines are never driven high at all.
            self.sp = serial.Serial()
            self.sp.port = port
            self.sp.baudrate = baud
            self.sp.bytesize = serial.EIGHTBITS
            self.sp.parity = serial.PARITY_NONE
            self.sp.stopbits = serial.STOPBITS_ONE
            self.sp.xonxoff = False          # Handshake.None
            self.sp.rtscts = False
            self.sp.dsrdtr = False
            self.sp.dtr = False
            self.sp.rts = False
            self.sp.inter_byte_timeout = None
            self.sp.write_timeout = None     # .NET WriteTimeout: infinite
            # Host-side read timeout only; the vendor polls BytesToRead
            # instead, which amounts to the same non-blocking behaviour.
            self.sp.timeout = 0.05
            self.sp.open()
            try:
                self.sp.dtr = False
                self.sp.rts = False
            except Exception:
                pass                      # not every driver permits this
            # A 4 kB driver buffer is small against 4254-byte packets; the
            # vendor asks for 100000. Windows-only in pyserial.
            try:
                self.sp.set_buffer_size(rx_size=100000, tx_size=2048)
            except Exception:
                pass
        except Exception as exc:
            hint = ("If the port is busy, close the vendor ScopeMeter application -- "
                    "it holds\nthe port exclusively.")
            if not sys.platform.startswith("win"):
                hint += ("\nIf permission was denied, add yourself to the group owning "
                         "the device\n(often 'dialout' or 'uucp') and log back in.")
            sys.exit("could not open %s: %s\n%s" % (port, exc, hint))
        self.buf = bytearray()
        self.log = bytearray()
        self.verbose = verbose
        self._deep_run = 0
        time.sleep(0.2)
        self.sp.reset_input_buffer()

    # -- plumbing ---------------------------------------------------------

    def close(self):
        try:
            self.sp.close()
        except Exception:
            pass

    # A long uninterrupted run of deep-record requests is what drives the
    # instrument into the hard lock described in README.md -- the one that
    # needs a USB disconnect and power cycle. Command 3 returns it to live
    # mode and resets the count. This is a tripwire, not a cure: it warns
    # rather than injecting a command 3, because that would re-arm the
    # acquisition and discard a held trigger, which is exactly what --hold
    # exists to preserve.
    DEEP_RUN_WARN = 20

    # Minimum spacing between commands. The vendor application polls once a
    # second and reads continuously in between, and that cadence has been run
    # for 90 seconds against this instrument without provoking the hard hang --
    # whereas issuing commands back to back does provoke it. Pacing to match is
    # the one pattern actually demonstrated to be safe, so it is the default.
    MIN_COMMAND_INTERVAL = 1.0

    def send(self, cmd, param=0):
        gap = self.min_interval - (time.time() - self._last_send)
        if gap > 0:
            time.sleep(gap)
        self._last_send = time.time()
        if cmd == CMD_DEEP:
            self._deep_run += 1
            if self._deep_run == self.DEEP_RUN_WARN:
                print("warning: %d deep-record requests in a row with no live-mode "
                      "request between\n         them. That pattern has locked this "
                      "instrument hard enough to need a USB\n         disconnect and "
                      "power cycle. Something is polling the wrong command."
                      % self._deep_run, file=sys.stderr)
        elif cmd == CMD_SCREEN:
            self._deep_run = 0
        self.sp.write(build_command(cmd, param))

    def drain(self):
        time.sleep(0.05)
        while self.sp.read(65536):
            pass
        self.buf.clear()

    def read_packet(self, want, timeout=4.0):
        deadline = time.time() + timeout
        while time.time() < deadline:
            chunk = self.sp.read(65536)
            if chunk:
                self.buf += chunk
                self.log += chunk
            for off, ptype, pkt in iter_packets(self.buf):
                if ptype == want:
                    del self.buf[:off + 1]
                    return pkt
            if len(self.buf) > 262144:
                del self.buf[:131072]
        return None

    # -- operations -------------------------------------------------------

    def ping(self, timeout=2.0):
        """Return 0 for scope mode, non-zero for multimeter mode, None on no reply."""
        self.drain()
        for _ in range(3):
            self.send(CMD_PING)
            pkt = self.read_packet(PKT_HELLO, timeout / 3.0)
            if pkt is not None:
                return pkt[1]
        return None

    def ensure_live(self, tries=8):
        """Put the instrument in live mode, verifying that it actually replies.

        Never call this when the instrument may be holding a Single-shot
        acquisition; see avoid_live.

        Sending cmd 3 is not enough on its own -- after a deep-record or stored
        recall the instrument can need several goes, and a recall issued while
        it is not live simply wedges the serial interface for a while. So we
        keep going until a screen packet actually comes back.
        """
        if self.avoid_live:
            raise RuntimeError(
                "refusing to send a live-mode request: the instrument may be "
                "holding a\nSingle-shot acquisition, and asking it to go live from "
                "there hangs it.")
        for _ in range(tries):
            self.drain()
            self.send(CMD_SCREEN)
            pkt = self.read_packet(PKT_SCREEN, 2.5)
            if pkt is not None and len(pkt) >= SCREEN_PACKET_LEN:
                return True
            time.sleep(0.3)
        return False

    def acquire(self, deep=True, tries=25, settle=0.25, verbose=True):
        """Force a fresh acquisition and return a validated Record.

        Asking for the deep buffer (cmd 4) puts the instrument into a held
        remote state: it replays the same buffer forever and the front panel
        locks up. Sending cmd 3 returns it to live mode, which re-arms
        acquisition -- but for a few seconds afterwards the deep buffer is only
        partly filled, so we retry until a packet passes validation.
        """
        last = "no packet received"
        for attempt in range(tries):
            self.drain()
            self.send(CMD_SCREEN)                    # back to live -> re-arm
            pkt = self.read_packet(PKT_SCREEN, 2.5)
            if deep:
                time.sleep(settle)                   # let the deep buffer fill
                self.drain()
                self.send(CMD_DEEP)
                pkt = self.read_packet(PKT_DEEP, 4.0)
            if pkt is None:
                last = "no packet received"
            else:
                rec = decode_packet(pkt)
                if rec is None:
                    last = "packet did not decode"
                else:
                    ok, why = validate(rec)
                    if ok:
                        return rec
                    last = why
            if verbose and attempt and attempt % 4 == 0:
                print("    ... waiting for a complete acquisition (%s)" % last,
                      file=sys.stderr)
        raise RuntimeError("gave up after %d attempts: %s" % (tries, last))

    def fetch_held(self, deep=True, tries=6, timeout=4.0, quiet=False):
        """Read whatever is already in the acquisition buffer, without re-arming.

        With the instrument's trigger set to Normal, a trigger event is captured
        and then *held* -- repeated requests return the identical buffer until
        it is re-armed. That is exactly what you want for a one-shot event you
        cannot time by hand: arm the trigger, cause the event, then collect it
        whenever you get round to it.

        acquire() deliberately re-arms first, which would discard the very
        event you were waiting for, so this is a separate path. Packets are
        still validated, to reject a buffer that was never filled.
        """
        want = PKT_DEEP if deep else PKT_SCREEN
        cmd = CMD_DEEP if deep else CMD_SCREEN
        last = "no packet received"
        for _ in range(tries):
            self.drain()
            self.send(cmd)
            pkt = self.read_packet(want, timeout)
            if pkt is None:
                last = "no packet received"
                continue
            rec = decode_packet(pkt)
            if rec is None:
                last = "packet did not decode"
                continue
            # Structural checks only: the reported measurements describe what
            # the instrument sees now, not the held samples, so they are
            # expected to disagree. Flag a large gap rather than reject it --
            # it also means the volts/div may have moved since the trigger.
            ok, why = validate(rec, tol=None)
            if ok:
                worst = amplitude_agreement(rec)
                if not quiet and worst is not None and worst > 0.20:
                    print("note: the held samples disagree with the instrument's "
                          "current readings by %.0f%%.\n      That is normal for a "
                          "held buffer, but the scaling comes from those readings --\n"
                          "      so do not change volts/div between triggering and "
                          "collecting." % (worst * 100), file=sys.stderr)
                return rec
            last = why
        raise RuntimeError("could not read the held buffer: %s" % last)

    def fetch_stored(self, slot, timeout=4.0):
        """Recall a waveform saved on the instrument. Returns a Record or None."""
        if not self.ensure_live():
            raise RuntimeError("could not get the instrument into live mode; "
                               "power-cycle it and try again")
        self.drain()
        self.send(CMD_LOAD_STORED, slot)
        pkt = self.read_packet(PKT_STORED, timeout)
        if pkt is None:
            return None
        return decode_packet(pkt)

    def release(self):
        """Hand the instrument back to its front panel before disconnecting.

        Does nothing when avoid_live is set. Asking the instrument to go live
        while it is holding a Single-shot acquisition hangs its firmware --
        reproducibly -- so anything working with a held trigger must not send
        command 3 at all, release included. There is nothing to hand back in
        that case anyway: the instrument was never taken out of the state the
        user put it in.

        The front panel is unresponsive for as long as the host keeps issuing
        commands; it comes back by itself shortly after the traffic stops. So
        the release is: leave it in live mode rather than holding a deep record
        or a recalled waveform, read the reply so nothing is left half-sent,
        and then stop talking to it. Retrying in a loop here would be actively
        counterproductive -- each extra command re-freezes the panel, and the
        last one would leave it freshly frozen on the way out.
        """
        if self.avoid_live:
            return True
        try:
            # Two spaced live-mode requests, not a tight retry loop. One is
            # sometimes not enough to get the instrument out of remote hold
            # after a lot of deep-record traffic; hammering it is worse than
            # useless, because each command re-freezes the panel and the last
            # one would leave it freshly frozen on the way out.
            for _ in range(2):
                self.drain()
                self.send(CMD_SCREEN)
                self.read_packet(PKT_SCREEN, 2.0)
                time.sleep(0.3)
            return True
        except Exception:
            return False

    # -- context manager, so a crash cannot leave the instrument held --------

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc, tb):
        self.release()
        self.close()
        return False
