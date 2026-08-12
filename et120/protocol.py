"""Wire protocol for the ET120MC2 / ET120 handheld oscilloscope.

Constants and framing only -- nothing here talks to a serial port or decodes a
waveform. See PROTOCOL.md for how this was established.
"""

SYNC = 0xA5

# Commands (host -> instrument). All are data requests or stream selections;
# there is no command that changes instrument settings.
CMD_PING = 1          # -> 0x21 hello; payload 0 = scope mode, non-zero = DMM
CMD_LOAD_STORED = 2   # param = slot number -> 0x22
CMD_SCREEN = 3        # -> 0x23, 600 bytes/ch of screen data (min/max pairs)
CMD_DEEP = 4          # -> 0x24, 2048 bytes/ch of ADC data   <-- the useful one

PKT_HELLO = 0x21
PKT_STORED = 0x22     # waveform saved on the instrument, recalled by slot
PKT_SCREEN = 0x23
PKT_DEEP = 0x24

STORED_SLOT_MAX = 200      # the vendor dialog caps the slot number here

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

# Screen record (0x23 and 0x22): display coordinates, 8 divisions over 200
# pixels, y increasing downwards. No absolute zero reference -- it moves with
# the vertical position control -- so we anchor it to the scope's own -Vpeak.
SCREEN_COUNTS_PER_DIV = 25.0
SCREEN_MIN, SCREEN_MAX = 0, 200

# The 2048-point record spans 16.384 divisions -> 125 transmitted samples per
# division. Only every 5th is a real sample (see the zero-order-hold note in
# process.py).
DEEP_SAMPLES_PER_DIV = 125.0
# 600 bytes = 300 screen columns of (min, max) over 10 divisions.
SCREEN_COLUMNS_PER_DIV = 30.0

MAX_PACKET_BODY = 5000     # the vendor application's own cap


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
    application's 'tempPackage' layout so the documented field offsets line up.
    Resynchronisation is by scanning for SYNC, so a bad checksum simply means
    "keep looking".
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
                if length <= MAX_PACKET_BODY and end < n:
                    total = (SYNC + ptype + buf[i + 2] + buf[i + 3] + buf[i + 4]
                             + sum(buf[i + 5:end + 1])) & 0xFF
                    if total == 0:
                        yield i, ptype, bytes([ptype]) + bytes(buf[i + 5:end])
                        i = end + 1
                        continue
        i += 1
