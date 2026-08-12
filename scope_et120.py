#!/usr/bin/env python3
"""
scope_et120.py -- dump raw waveform data from an ET120MC2 / ET120 handheld
                  oscilloscope over its USB serial port, and export it in
                  formats LTspice can use.

The vendor Windows application is the only thing that normally talks to this
scope, and it is unreliable (see LIMITATIONS / PROTOCOL.md).  This script talks
to the instrument directly.

Quick start
-----------
    python scope_et120.py ports                    # find the scope
    python scope_et120.py info                     # connect, report one acquisition
    python scope_et120.py capture -o signal        # -> signal.pwl for LTspice
    python scope_et120.py capture -o signal --all  # -> .pwl + .csv + .raw
    python scope_et120.py capture -n 10 -o run     # 10 separate records
    python scope_et120.py sniff -o dump.bin        # log raw serial traffic
    python scope_et120.py decode dump.bin -o sig   # decode a dump offline

Drive an LTspice circuit with the captured waveform by setting a voltage
source's value to:

    PWL file=C:\\path\\to\\signal.pwl

The .raw file is an LTspice ASCII waveform file -- open it in the LTspice
waveform viewer to look at the capture, but use the .pwl to drive a circuit.

Requires: pyserial, numpy.
"""

import argparse
import os
import struct
import sys
import time

try:
    import numpy as np
except ImportError:
    sys.exit("numpy is required:  python -m pip install numpy")

__version__ = "1.0"

LIMITATIONS = """\
LIMITATIONS -- what this scope can and cannot give you
------------------------------------------------------
* 410 real sample points per capture. Always. The scope transmits a 2048-byte
  record but it is 410 real samples with each one repeated 5 times (zero-order
  hold), at every timebase setting from 5 ns/div to 50 ms/div. There is no
  deeper memory to ask for; this is the hardware limit.

* The timebase is the only trade-off you have:
      record length = 16.384 * secs_per_div      (the record is 16.384 div long)
      sample rate   = 25 / secs_per_div
  e.g.  5 ms/div -> 5 kSa/s over 82 ms      1 ms/div -> 25 kSa/s over 16.4 ms
       20 us/div -> 1.25 MSa/s over 328 us

* 8-bit vertical resolution, and only ~10 divisions of range, so one code is
  volts_per_div/25.5. Small signals should be zoomed in with the V/div control
  before capturing, not scaled up afterwards.

* Captures are snapshots, not a continuous stream. Each one takes ~1.3 s of
  wall time (re-arm, wait for the buffer to fill, transfer 4.3 kB at 115200
  baud). Consecutive captures are NOT contiguous in time -- the scope
  re-triggers in between -- so they must not be concatenated into one signal.

* Requesting the deep record freezes the scope's front panel until it is put
  back in live mode. This script does that automatically on exit.

* At timebases faster than the ADC's real-time rate the scope must be using
  equivalent-time sampling, so fast-timebase records are only meaningful for
  repetitive signals. The nominal time axis above still applies.

* Records are short, so plan around snapshots. A sound card gives 48 kSa/s
  continuously, but only for sources it can load: a piezo pickup is a few nF
  of capacitance, and a ~10 kOhm line input high-passes it at 1/(2*pi*R*C)
  ~ 8 kHz, burying a 41 Hz low E some 46 dB down. A x10 scope probe presents
  10 MOhm and moves that corner to ~8 Hz, so for high-impedance sources this
  instrument is the right front end despite the short records.
"""


# ---------------------------------------------------------------------------
# Protocol constants (see PROTOCOL.md)
# ---------------------------------------------------------------------------

SYNC = 0xA5

CMD_PING = 1          # -> 0x21 hello; payload 0 = scope mode, non-zero = DMM
CMD_LOAD_STORED = 2   # param = stored waveform slot
CMD_SCREEN = 3        # -> 0x23, 600 bytes/ch of screen data (min/max pairs)
CMD_DEEP = 4          # -> 0x24, 2048 bytes/ch of ADC data   <-- the useful one

PKT_HELLO = 0x21
PKT_SCREEN = 0x23
PKT_DEEP = 0x24

# volts/div, indexed by the channel block's byte 0 (voltaBaseF[] in the vendor source)
VOLTS_PER_DIV = [
    0.01, 0.02, 0.05, 0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0,
    0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 50.0, 100.0,
    1.0, 2.0, 5.0, 10.0, 20.0, 50.0, 100.0, 200.0, 500.0, 1000.0,
]

# seconds/div, indexed by metadata byte 2 (timeBaseF[] in the vendor source)
SECS_PER_DIV = [
    0.0, 5e-9, 1e-8, 2.5e-8, 5e-8, 1e-7, 2e-7, 5e-7, 1e-6, 2e-6, 5e-6,
    1e-5, 2e-5, 5e-5, 1e-4, 2e-4, 5e-4, 1e-3, 2e-3, 5e-3, 1e-2, 2e-2,
    5e-2, 0.1, 0.2, 0.5, 1.0, 2.0, 5.0, 10.0, 20.0, 50.0,
]
SECS_PER_DIV_S = [
    "-", "5ns", "10ns", "25ns", "50ns", "100ns", "200ns", "500ns", "1us", "2us",
    "5us", "10us", "20us", "50us", "100us", "200us", "500us", "1ms", "2ms",
    "5ms", "10ms", "20ms", "50ms", "100ms", "200ms", "500ms", "1s", "2s",
    "5s", "10s", "20s", "50s",
]

# Deep record (0x24): ADC codes. 0 V sits at code 128 and the full 8-bit range
# spans 10 vertical divisions, so one division is 25.5 codes.
DEEP_ZERO_CODE = 128.0
DEEP_COUNTS_PER_DIV = 25.5

# Screen record (0x23): display coordinates, 8 divisions over 200 pixels,
# y increasing downwards. No absolute zero reference -- it moves with the
# vertical position control -- so we anchor it to the scope's own -Vpeak.
SCREEN_COUNTS_PER_DIV = 25.0
SCREEN_MIN, SCREEN_MAX = 0, 200

# The 2048-point record spans 16.384 divisions -> 125 transmitted samples per
# division. Only every 5th is a real sample (see LIMITATIONS).
DEEP_SAMPLES_PER_DIV = 125.0
# 600 bytes = 300 screen columns of (min, max) over 10 divisions.
SCREEN_COLUMNS_PER_DIV = 30.0


# ---------------------------------------------------------------------------
# Framing
# ---------------------------------------------------------------------------

def build_command(cmd, param=0):
    """A5 <cmd> <param> <chk>, where all four bytes sum to 0 (mod 256)."""
    frame = bytearray([SYNC, cmd & 0xFF, param & 0xFF, 0])
    frame[3] = (-(frame[0] + frame[1] + frame[2])) & 0xFF
    return bytes(frame)


def iter_packets(buf):
    """Yield (offset, type, packet) for every checksum-valid packet in buf.

    Short packets:  A5 TYPE PARAM CHK
    Long packets:   A5 TYPE LEN_LO LEN_HI HCHK <LEN bytes> DCHK

    The yielded packet is TYPE followed by the body, matching the vendor
    application's 'tempPackage' layout so the field offsets below line up.
    """
    i, n = 0, len(buf)
    while i <= n - 4:                  # shortest packet (0x21 hello) is 4 bytes
        if buf[i] != SYNC:
            i += 1
            continue
        ptype = buf[i + 1]
        if ptype == PKT_HELLO:
            if (SYNC + ptype + buf[i + 2] + buf[i + 3]) & 0xFF == 0:
                yield i, ptype, bytes([ptype, buf[i + 2]])
                i += 4
                continue
        elif ptype >= 0x22:
            if i + 5 <= n and (SYNC + ptype + buf[i + 2] + buf[i + 3] + buf[i + 4]) & 0xFF == 0:
                length = buf[i + 2] | (buf[i + 3] << 8)
                end = i + 5 + length
                if length <= 5000 and end < n:
                    total = (SYNC + ptype + buf[i + 2] + buf[i + 3] + buf[i + 4]
                             + sum(buf[i + 5:end + 1])) & 0xFF
                    if total == 0:
                        yield i, ptype, bytes([ptype]) + bytes(buf[i + 5:end])
                        i = end + 1
                        continue
        i += 1


# ---------------------------------------------------------------------------
# Packet decoding
# ---------------------------------------------------------------------------

class Channel(object):
    """One acquisition channel out of a waveform packet."""

    def __init__(self):
        self.index = 0
        self.raw = None              # uint8 samples exactly as transmitted
        self.volts_per_div = 0.0
        self.probe_exp = 0
        self.zero_code = DEEP_ZERO_CODE
        self.counts_per_div = DEEP_COUNTS_PER_DIV
        self.inverted = False        # screen records count downwards
        self.clipped = False
        # what the instrument itself reports for this channel
        self.rep_vrms = 0.0
        self.rep_vpp = 0.0
        self.rep_vp_pos = 0.0
        self.rep_vp_neg = 0.0
        self.rep_period = 0.0
        self.rep_freq = 0.0

    def volts(self):
        codes = self.raw.astype(np.float64)
        delta = (self.zero_code - codes) if self.inverted else (codes - self.zero_code)
        return delta / self.counts_per_div * self.volts_per_div

    def span_volts(self):
        """Peak-to-peak from the raw codes, independent of any zero reference."""
        return (float(self.raw.max()) - float(self.raw.min())) \
            / self.counts_per_div * self.volts_per_div


class Record(object):
    """One decoded waveform packet."""

    def __init__(self):
        self.deep = True
        self.timebase_index = 0
        self.secs_per_div = 0.0
        self.channels = {}           # 1-based channel number -> Channel
        self.dt = 0.0                # seconds between consecutive samples
        self.zoh = 1                 # zero-order-hold repeat factor removed

    @property
    def npoints(self):
        return len(next(iter(self.channels.values())).raw)

    @property
    def duration(self):
        return self.npoints * self.dt


def _parse_channel_block(block, index):
    ch = Channel()
    ch.index = index
    vdiv_idx = block[0]
    ch.probe_exp = block[4]
    base = VOLTS_PER_DIV[vdiv_idx] if vdiv_idx < len(VOLTS_PER_DIV) else 1.0
    ch.volts_per_div = base * (10.0 ** ch.probe_exp)
    ch.rep_vrms = struct.unpack_from("<f", block, 25)[0]
    ch.rep_vpp = struct.unpack_from("<f", block, 29)[0]
    ch.rep_vp_pos = struct.unpack_from("<f", block, 37)[0]
    ch.rep_vp_neg = struct.unpack_from("<f", block, 41)[0]
    ch.rep_period = struct.unpack_from("<f", block, 45)[0]
    ch.rep_freq = struct.unpack_from("<f", block, 49)[0]
    return ch


def decode_packet(pkt):
    """Decode a 0x24 (deep, 2048 pt) or 0x23 (screen, 600 pt) packet."""
    ptype = pkt[0]
    if ptype == PKT_DEEP:
        nsamp, meta_off, deep = 2048, 4098, True
    elif ptype == PKT_SCREEN:
        nsamp, meta_off, deep = 600, 1202, False
    else:
        return None
    if len(pkt) < meta_off + 156:
        return None

    mask = pkt[1]
    if mask == 0:
        return None

    rec = Record()
    rec.deep = deep
    meta = pkt[meta_off:]
    rec.timebase_index = meta[2]
    if rec.timebase_index >= len(SECS_PER_DIV) or rec.timebase_index == 0:
        return None
    rec.secs_per_div = SECS_PER_DIV[rec.timebase_index]

    for ci, blk_off in ((0, 10), (1, 83)):
        if not (mask & (1 << ci)):
            continue
        ch = _parse_channel_block(meta[blk_off:blk_off + 73], ci + 1)
        start = 2 + nsamp * ci
        ch.raw = np.frombuffer(pkt[start:start + nsamp], dtype=np.uint8).copy()
        if deep:
            ch.zero_code = DEEP_ZERO_CODE
            ch.counts_per_div = DEEP_COUNTS_PER_DIV
        else:
            # Screen coordinates: gain is known but the zero moves with the
            # vertical position knob, so anchor the most-negative sample to
            # the -Vpeak the scope reports.
            ch.counts_per_div = SCREEN_COUNTS_PER_DIV
            ch.inverted = True
            ch.zero_code = (float(ch.raw.max())
                            + ch.rep_vp_neg * SCREEN_COUNTS_PER_DIV / ch.volts_per_div)
            ch.clipped = bool(ch.raw.min() <= SCREEN_MIN or ch.raw.max() >= SCREEN_MAX)
        rec.channels[ci + 1] = ch

    if not rec.channels:
        return None

    if deep:
        rec.dt = rec.secs_per_div / DEEP_SAMPLES_PER_DIV
    else:
        rec.dt = rec.secs_per_div / SCREEN_COLUMNS_PER_DIV / 2.0
    return rec


# ---------------------------------------------------------------------------
# Validation
#
# For a few seconds after being re-armed the scope hands back a half-filled
# deep buffer. Reject those by cross-checking the decoded peak-to-peak against
# the instrument's own Vpp readout, which it computes from full-resolution data.
# ---------------------------------------------------------------------------

def validate(rec, tol=0.20):
    """Return (ok, reason)."""
    for num, ch in rec.channels.items():
        raw = ch.raw
        tail = raw[int(len(raw) * 0.6):]
        if np.count_nonzero(tail) == 0:
            return False, "ch%d: buffer tail is all zeros (acquisition incomplete)" % num
        if raw.max() == raw.min():
            return False, "ch%d: flat buffer" % num
        got, want = ch.span_volts(), ch.rep_vpp
        if want > 1e-9:
            err = abs(got - want) / want
            if err > tol:
                return False, ("ch%d: decoded Vpp %.4g V vs scope's %.4g V (%.0f%% off)"
                               % (num, got, want, err * 100))
    return True, "ok"


# ---------------------------------------------------------------------------
# Zero-order-hold removal
# ---------------------------------------------------------------------------

def detect_zoh(raw, max_factor=16, threshold=0.90):
    """Find the repeat factor and phase of the scope's sample padding.

    Returns (factor, phase, confidence). factor == 1 means no padding found.
    """
    best = (1, 0, 0.0)
    n = len(raw)
    for f in range(2, max_factor + 1):
        if n // f < 16:
            break
        for phase in range(f):
            end = phase + (n - phase) // f * f
            groups = raw[phase:end].reshape(-1, f)
            const = np.count_nonzero(groups.max(axis=1) == groups.min(axis=1)) \
                / float(groups.shape[0])
            if const > best[2] + 1e-9:
                best = (f, phase, const)
    factor, phase, score = best
    if score < threshold:
        return 1, 0, score
    return factor, phase, score


def strip_zoh(rec):
    """Collapse the padding in place, recovering the real samples.

    Linear interpolation between real samples (what LTspice does with a PWL
    source) reconstructs the signal far better than the 5x staircase, which
    would inject spurious high-frequency content into a simulation.
    """
    first = next(iter(rec.channels.values()))
    factor, phase, score = detect_zoh(first.raw)
    if factor <= 1:
        rec.zoh = 1
        return 1, score
    mid = phase + factor // 2
    for ch in rec.channels.values():
        ch.raw = ch.raw[mid::factor].copy()
    rec.dt *= factor
    rec.zoh = factor
    return factor, score


# ---------------------------------------------------------------------------
# Serial transport
# ---------------------------------------------------------------------------

class Scope(object):
    def __init__(self, port, baud=115200, verbose=False):
        try:
            import serial
        except ImportError:
            sys.exit("pyserial is required:  python -m pip install pyserial")
        self.port_name = port
        try:
            self.sp = serial.Serial(port, baud, timeout=0.05)
        except Exception as exc:
            sys.exit("could not open %s: %s\n"
                     "(if the port is busy, close the vendor ScopeMeter application "
                     "-- it holds the port exclusively)" % (port, exc))
        self.buf = bytearray()
        self.log = bytearray()
        self.verbose = verbose
        time.sleep(0.2)
        self.sp.reset_input_buffer()

    def close(self):
        try:
            self.sp.close()
        except Exception:
            pass

    def send(self, cmd, param=0):
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

    def ping(self, timeout=2.0):
        """Return 0 for scope mode, non-zero for multimeter mode, None on no reply."""
        self.drain()
        for _ in range(3):
            self.send(CMD_PING)
            pkt = self.read_packet(PKT_HELLO, timeout / 3.0)
            if pkt is not None:
                return pkt[1]
        return None

    def acquire(self, deep=True, tries=25, settle=0.25, verbose=True):
        """Force a fresh acquisition and return a validated Record.

        Asking for the deep buffer (cmd 4) puts the scope into a held remote
        state: it replays the same buffer forever and the front panel locks up.
        Sending cmd 3 returns it to live mode, which re-arms acquisition -- but
        for a few seconds afterwards the deep buffer is only partly filled, so
        we retry until a packet passes validation.
        """
        want = PKT_DEEP if deep else PKT_SCREEN
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

    def release(self):
        """Return the scope to live mode so its front panel works again."""
        try:
            self.send(CMD_SCREEN)
            time.sleep(0.2)
        except Exception:
            pass


# ---------------------------------------------------------------------------
# Measurement / reporting
# ---------------------------------------------------------------------------

def measure(volts, dt):
    v = np.asarray(volts, dtype=np.float64)
    out = {
        "vmin": float(v.min()), "vmax": float(v.max()),
        "vpp": float(v.max() - v.min()), "vmean": float(v.mean()),
        "vrms": float(np.sqrt((v ** 2).mean())), "freq": 0.0,
    }
    if len(v) >= 32 and dt > 0:
        z = v - v.mean()
        spec = np.abs(np.fft.rfft(z * np.hanning(len(z))))
        if len(spec) > 3:
            b = int(np.argmax(spec[1:]) + 1)
            if 0 < b < len(spec) - 1:
                y0, y1, y2 = spec[b - 1], spec[b], spec[b + 1]
                denom = y0 - 2 * y1 + y2
                bb = b + (0.5 * (y0 - y2) / denom if denom != 0 else 0.0)
                if bb > 0:
                    out["freq"] = float(bb / (len(v) * dt))
    return out


def describe(rec, stream=sys.stdout):
    tb = (SECS_PER_DIV_S[rec.timebase_index]
          if rec.timebase_index < len(SECS_PER_DIV_S) else "?")
    kind = "deep record" if rec.deep else "screen record (min/max peak detect)"
    print("  %s at %s/div" % (kind, tb), file=stream)
    print("  %d points over %.6g s   dt = %.6g s   %.6g Sa/s%s"
          % (rec.npoints, rec.duration, rec.dt, 1.0 / rec.dt if rec.dt else 0.0,
             "" if rec.zoh <= 1 else "   [%dx padding removed]" % rec.zoh),
          file=stream)
    for num in sorted(rec.channels):
        ch = rec.channels[num]
        m = measure(ch.volts(), rec.dt)
        print("  CH%d  %.6g V/div (probe x%d)"
              % (num, ch.volts_per_div, 10 ** ch.probe_exp), file=stream)
        print("       decoded  Vpp %-9.4g +Vp %-9.4g -Vp %-9.4g Vrms %-9.4g f %.6g Hz"
              % (m["vpp"], m["vmax"], m["vmin"], m["vrms"], m["freq"]), file=stream)
        print("       scope    Vpp %-9.4g +Vp %-9.4g -Vp %-9.4g Vrms %-9.4g f %.6g Hz"
              % (ch.rep_vpp, ch.rep_vp_pos, ch.rep_vp_neg, ch.rep_vrms, ch.rep_freq),
              file=stream)
        if ch.clipped:
            print("       WARNING: trace is clipped at the edge of the display; "
                  "voltages are wrong.", file=stream)
        if not rec.deep:
            print("       NOTE: screen record -- the zero reference is inferred from "
                  "the scope's -Vp.", file=stream)
        nyq = 0.5 / rec.dt if rec.dt else 0.0
        if m["freq"] > 0.4 * nyq > 0:
            print("       WARNING: %.6g Hz is near Nyquist (%.6g Hz) -- the capture is "
                  "likely aliased.\n"
                  "                Use a faster timebase." % (m["freq"], nyq), file=stream)


# ---------------------------------------------------------------------------
# Exporters
# ---------------------------------------------------------------------------

def write_pwl(path, t, v, repeat=1):
    """LTspice piecewise-linear source file:  'V1 in 0 PWL file=<path>'.

    Always writes '.' as the decimal separator regardless of system locale.
    """
    step = (t[1] - t[0]) if len(t) > 1 else 0.0
    period = t[-1] + step
    with open(path, "w", newline="\n") as fh:
        fh.write("; ET120MC2 capture -- %d points, dt=%.9g s, length=%.9g s\n"
                 % (len(t), step, period))
        if repeat > 1:
            fh.write("; tiled %d times -> total %.9g s\n" % (repeat, period * repeat))
        for r in range(repeat):
            base = r * period
            for ti, vi in zip(t, v):
                fh.write("%.9e\t%.9e\n" % (base + ti, vi))
    return path


def write_csv(path, t, channels):
    with open(path, "w", newline="\n") as fh:
        fh.write("time," + ",".join(name for name, _ in channels) + "\n")
        cols = [vals for _, vals in channels]
        for i in range(len(t)):
            fh.write("%.9e" % t[i])
            for c in cols:
                fh.write(",%.9e" % c[i])
            fh.write("\n")
    return path


def write_ltspice_raw(path, t, channels, title="ET120MC2 capture"):
    """LTspice ASCII .raw -- open directly in the LTspice waveform viewer.

    This is a waveform file for viewing and measuring. To *drive* a circuit,
    use the .pwl file with 'PWL file=...' on a voltage source.
    """
    npts = len(t)
    names = ["V(%s)" % name for name, _ in channels]
    with open(path, "w", newline="\n") as fh:
        fh.write("Title: * %s\n" % title)
        fh.write("Date: %s\n" % time.strftime("%a %b %d %H:%M:%S %Y"))
        fh.write("Plotname: Transient Analysis\n")
        fh.write("Flags: real forward\n")
        fh.write("No. Variables: %d\n" % (1 + len(names)))
        fh.write("No. Points: %d\n" % npts)
        fh.write("Offset: 0.0000000000000000e+000\n")
        fh.write("Command: scope_et120.py %s\n" % __version__)
        fh.write("Variables:\n")
        fh.write("\t0\ttime\ttime\n")
        for i, nm in enumerate(names):
            fh.write("\t%d\t%s\tvoltage\n" % (i + 1, nm))
        fh.write("Values:\n")
        cols = [vals for _, vals in channels]
        for i in range(npts):
            fh.write("%d\t%.15e\n" % (i, t[i]))
            for c in cols:
                fh.write("\t%.15e\n" % c[i])
    return path


def export(rec, stem, want_pwl=True, want_csv=False, want_raw=False,
           channel=None, repeat=1, quiet=False, remove_dc=False):
    t = np.arange(rec.npoints, dtype=np.float64) * rec.dt

    nums = sorted(rec.channels)
    if channel is not None:
        if channel not in rec.channels:
            raise SystemExit("CH%d is not active in this capture (active: %s)"
                             % (channel, ", ".join("CH%d" % c for c in nums)))
        nums = [channel]

    named = []
    for n in nums:
        v = rec.channels[n].volts()
        if remove_dc:
            offset = v.mean()
            v = v - offset
            if not quiet:
                print("  ch%d: removed %.4g V DC offset" % (n, offset))
        named.append(("ch%d" % n, v))
    written = []
    if want_pwl:
        for name, v in named:
            path = "%s.pwl" % stem if len(named) == 1 else "%s_%s.pwl" % (stem, name)
            written.append(write_pwl(path, t, v, repeat=repeat))
    if want_csv:
        written.append(write_csv("%s.csv" % stem, t, named))
    if want_raw:
        written.append(write_ltspice_raw("%s.raw" % stem, t, named))
    if not quiet:
        for p in written:
            print("  wrote %s" % p)
    return written


# ---------------------------------------------------------------------------
# Commands
# ---------------------------------------------------------------------------

def cmd_ports(args):
    try:
        from serial.tools import list_ports
    except ImportError:
        sys.exit("pyserial is required:  python -m pip install pyserial")
    found = list(list_ports.comports())
    if not found:
        print("no serial ports found")
        return 1
    for p in found:
        note = "   <-- likely the scope" if p.vid == 0x28E9 else ""
        print("%-8s %s%s" % (p.device, p.description, note))
    return 0


def _connect(args):
    scope = Scope(args.port, verbose=args.verbose)
    mode = scope.ping()
    if mode is None:
        scope.close()
        sys.exit("no reply on %s.\n"
                 "Check that the scope is powered on and connected, that this is the "
                 "right port\n(try: python scope_et120.py ports), and that the vendor "
                 "application is closed." % args.port)
    if mode != 0:
        print("note: the instrument answered in multimeter (DMM) mode. Switch it to "
              "scope mode\n      to capture waveforms.", file=sys.stderr)
    return scope


def _prep(rec, args):
    if not args.keep_zoh and rec.deep:
        strip_zoh(rec)
    return rec


def cmd_info(args):
    scope = _connect(args)
    try:
        print("connected on %s" % args.port)
        rec = _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
        describe(rec)
    finally:
        scope.release()
        scope.close()
    return 0


def _acquire_best(scope, args, n):
    """Take n acquisitions and keep the one with the largest peak-to-peak.

    Useful for transients you cannot time by hand -- an instrument pluck, a
    switching event -- where most snapshots catch silence.
    """
    if n <= 1:
        return _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
    best, seen = None, set()
    for k in range(n):
        rec = _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
        vpp = max(ch.span_volts() for ch in rec.channels.values())
        seen.add(next(iter(rec.channels.values())).raw.tobytes())
        print("    candidate %d/%d: Vpp %.4g V" % (k + 1, n, vpp))
        if best is None or vpp > max(c.span_volts() for c in best.channels.values()):
            best = rec
    if len(seen) == 1:
        print("    WARNING: all %d candidates were identical. The scope is probably\n"
              "             not re-triggering -- check the trigger mode and level."
              % n, file=sys.stderr)
    return best


def cmd_capture(args):
    scope = _connect(args)
    stem = os.path.splitext(args.output)[0] if args.output else "capture"
    want_pwl = args.all or args.pwl or not (args.csv or args.raw)
    try:
        for i in range(args.count):
            label = stem if args.count == 1 else "%s_%02d" % (stem, i)
            print("capture %d/%d ..." % (i + 1, args.count))
            rec = _acquire_best(scope, args, args.best_of)
            describe(rec)
            export(rec, label, want_pwl, args.all or args.csv, args.all or args.raw,
                   channel=args.channel, repeat=args.repeat, remove_dc=args.remove_dc)
            if args.dump:
                with open("%s.bin" % label, "wb") as fh:
                    fh.write(scope.log)
                print("  wrote %s.bin (raw serial log)" % label)
                scope.log = bytearray()
    finally:
        scope.release()
        scope.close()
    if args.count > 1:
        print("\nNote: these records are NOT contiguous in time -- the scope re-triggers\n"
              "      between captures. Do not concatenate them into one signal.")
    return 0


def cmd_decode(args):
    with open(args.dumpfile, "rb") as fh:
        data = fh.read()
    packets = [p for _, t, p in iter_packets(data) if t in (PKT_DEEP, PKT_SCREEN)]
    if not packets:
        sys.exit("no waveform packets found in %s" % args.dumpfile)
    good = []
    for pkt in packets:
        rec = decode_packet(pkt)
        if rec is not None and validate(rec)[0]:
            good.append(rec)
    print("%s: %d waveform packet(s), %d valid" % (args.dumpfile, len(packets), len(good)))
    if not good:
        sys.exit("no packet passed validation (all were stale or incomplete buffers)")
    deep = [r for r in good if r.deep]
    chosen = _prep((deep or good)[0], args)
    describe(chosen)
    if args.output:
        want_pwl = args.all or args.pwl or not (args.csv or args.raw)
        export(chosen, os.path.splitext(args.output)[0], want_pwl,
               args.all or args.csv, args.all or args.raw,
               channel=args.channel, repeat=args.repeat, remove_dc=args.remove_dc)
    return 0


def cmd_sniff(args):
    scope = _connect(args)
    print("logging raw traffic for %.1f s (polling with command %d) ..."
          % (args.seconds, args.cmd))
    try:
        end, nxt = time.time() + args.seconds, 0.0
        while time.time() < end:
            if time.time() >= nxt:
                scope.send(args.cmd)
                nxt = time.time() + args.interval
            chunk = scope.sp.read(65536)
            if chunk:
                scope.log += chunk
    finally:
        scope.release()
        scope.close()
    with open(args.output, "wb") as fh:
        fh.write(scope.log)
    print("wrote %s (%d bytes)" % (args.output, len(scope.log)))
    for off, ptype, pkt in iter_packets(scope.log):
        print("  @%-8d type 0x%02x  %d bytes" % (off, ptype, len(pkt)))
    return 0


def cmd_limits(args):
    print(LIMITATIONS)
    return 0


def main(argv=None):
    ap = argparse.ArgumentParser(
        prog="scope_et120.py",
        description="Dump raw waveform data from an ET120MC2 oscilloscope and export "
                    "it for LTspice.",
        epilog="Run 'scope_et120.py limits' for what the hardware can and cannot do.")
    ap.add_argument("-p", "--port", default="COM5", help="serial port (default: COM5)")
    ap.add_argument("-v", "--verbose", action="store_true")
    ap.add_argument("--version", action="version", version="scope_et120.py " + __version__)
    sub = ap.add_subparsers(dest="cmd")

    def add_acq_opts(p):
        p.add_argument("--screen", action="store_true",
                       help="use the 600-point screen record instead of the "
                            "2048-point deep record")
        p.add_argument("--settle", type=float, default=0.25,
                       help="seconds to let the deep buffer fill after re-arming")

    def add_export_opts(p):
        p.add_argument("-o", "--output", help="output file stem")
        p.add_argument("--pwl", action="store_true", help="write LTspice PWL (default)")
        p.add_argument("--csv", action="store_true", help="write CSV")
        p.add_argument("--raw", action="store_true", help="write LTspice ASCII .raw")
        p.add_argument("--all", action="store_true", help="write all three formats")
        p.add_argument("-c", "--channel", type=int, choices=(1, 2),
                       help="export only this channel")
        p.add_argument("--repeat", type=int, default=1, metavar="N",
                       help="tile the PWL N times for a longer transient run")
        p.add_argument("--remove-dc", action="store_true",
                       help="subtract the mean, so the waveform is centred on 0 V")
        p.add_argument("--keep-zoh", action="store_true",
                       help="keep the scope's 5x sample padding instead of collapsing it")

    p = sub.add_parser("ports", help="list serial ports")
    p.set_defaults(func=cmd_ports)

    p = sub.add_parser("info", help="connect and report one acquisition")
    add_acq_opts(p)
    p.add_argument("--keep-zoh", action="store_true")
    p.set_defaults(func=cmd_info)

    p = sub.add_parser("capture", help="capture waveform(s) and export")
    p.add_argument("-n", "--count", type=int, default=1, help="number of records")
    p.add_argument("--best-of", type=int, default=1, metavar="N",
                   help="take N acquisitions per record and keep the one with the "
                        "largest peak-to-peak (for catching plucks and other "
                        "transients you cannot time by hand)")
    p.add_argument("--dump", action="store_true", help="also save the raw serial log")
    add_acq_opts(p)
    add_export_opts(p)
    p.set_defaults(func=cmd_capture)

    p = sub.add_parser("decode", help="decode a saved raw serial dump offline")
    p.add_argument("dumpfile")
    add_export_opts(p)
    p.add_argument("--screen", action="store_true", help=argparse.SUPPRESS)
    p.set_defaults(func=cmd_decode)

    p = sub.add_parser("sniff", help="log raw serial traffic to a file")
    p.add_argument("-o", "--output", default="dump.bin")
    p.add_argument("--cmd", type=int, default=CMD_DEEP, help="command byte to poll with")
    p.add_argument("--interval", type=float, default=1.0)
    p.add_argument("--seconds", type=float, default=6.0)
    p.set_defaults(func=cmd_sniff)

    p = sub.add_parser("limits", help="print what the hardware can and cannot do")
    p.set_defaults(func=cmd_limits)

    args = ap.parse_args(argv)
    if not getattr(args, "cmd", None):
        ap.print_help()
        return 1
    return args.func(args)


if __name__ == "__main__":
    sys.exit(main())
