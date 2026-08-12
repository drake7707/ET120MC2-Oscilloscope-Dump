"""Turn waveform packets into calibrated volts."""

import struct
import sys

try:
    import numpy as np
except ImportError:
    sys.exit("numpy is required:  python -m pip install numpy")

from .protocol import (
    DEEP_COUNTS_PER_DIV, DEEP_SAMPLES_PER_DIV, DEEP_ZERO_CODE,
    PKT_DEEP, PKT_SCREEN, PKT_STORED,
    SCREEN_COLUMNS_PER_DIV, SCREEN_COUNTS_PER_DIV, SCREEN_MAX, SCREEN_MIN,
    SECS_PER_DIV, VOLTS_PER_DIV,
)


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

    def range_used(self):
        """Fraction of the 8-bit code range the capture occupies."""
        return (float(self.raw.max()) - float(self.raw.min())) / 255.0


class Record(object):
    """One decoded waveform packet."""

    def __init__(self):
        self.deep = True
        self.timebase_index = 0
        self.secs_per_div = 0.0
        self.channels = {}           # 1-based channel number -> Channel
        self.dt = 0.0                # seconds between consecutive samples
        self.zoh = 1                 # zero-order-hold repeat factor removed
        self.slot = None             # set for waveforms recalled from the instrument

    @property
    def npoints(self):
        return len(next(iter(self.channels.values())).raw)

    @property
    def duration(self):
        return self.npoints * self.dt


def _parse_channel_block(block, index):
    """The 73-byte per-channel parameter block inside the metadata."""
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
    """Decode a 0x24 (deep), 0x23 (screen) or 0x22 (stored) packet.

    Returns a Record, or None if the packet is not a waveform or is malformed.
    """
    ptype = pkt[0]
    slot = None
    if ptype == PKT_DEEP:
        nsamp, meta_off, deep = 2048, 4098, True
    elif ptype == PKT_SCREEN:
        nsamp, meta_off, deep = 600, 1202, False
    elif ptype == PKT_STORED:
        # 0x22 is a screen record behind a 4-byte prefix: type, slot, 00 06.
        # The vendor application drops those four bytes and parses the rest
        # exactly as a 0x23, which is what we do here.
        if len(pkt) < 4:
            return None
        slot = pkt[1]
        pkt = pkt[4:]
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
    rec.slot = slot
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
        if len(ch.raw) != nsamp:
            return None
        if deep:
            ch.zero_code = DEEP_ZERO_CODE
            ch.counts_per_div = DEEP_COUNTS_PER_DIV
        else:
            # Screen coordinates: the gain is known but the zero moves with the
            # vertical position control, so anchor the most-negative sample to
            # the -Vpeak the instrument reports.
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


def validate(rec, tol=0.20):
    """Reject stale or half-filled buffers. Returns (ok, reason).

    For a few seconds after being re-armed the instrument hands back a
    partly-filled deep buffer. Cross-checking the decoded peak-to-peak against
    the instrument's own Vpp readout -- which it computes from full-resolution
    data -- catches those, and doubles as a check that our scaling is right.

    Pass tol=None to run the structural checks only. That is needed when
    reading a *held* buffer, where the samples come from an earlier triggered
    acquisition while the reported measurements describe what the instrument
    is seeing now -- a disagreement there is expected, not a fault.
    """
    for num, ch in rec.channels.items():
        raw = ch.raw
        tail = raw[int(len(raw) * 0.6):]
        if np.count_nonzero(tail) == 0:
            return False, "ch%d: buffer tail is all zeros (acquisition incomplete)" % num
        if raw.max() == raw.min():
            return False, "ch%d: flat buffer" % num
        if tol is None:
            continue
        got, want = ch.span_volts(), ch.rep_vpp
        if want > 1e-9:
            err = abs(got - want) / want
            if err > tol:
                return False, ("ch%d: decoded Vpp %.4g V vs scope's %.4g V (%.0f%% off)"
                               % (num, got, want, err * 100))
    return True, "ok"


def amplitude_agreement(rec):
    """Largest relative disagreement between decoded and reported Vpp, or None."""
    worst = None
    for ch in rec.channels.values():
        if ch.rep_vpp > 1e-9:
            err = abs(ch.span_volts() - ch.rep_vpp) / ch.rep_vpp
            worst = err if worst is None else max(worst, err)
    return worst


def measure(volts, dt):
    """Basic measurements, including a parabolically-interpolated fundamental."""
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
