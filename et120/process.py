"""Signal conditioning applied to a decoded Record before export."""

import sys

try:
    import numpy as np
except ImportError:
    sys.exit("numpy is required:  python -m pip install numpy")

from .decode import detect_zoh, measure


def strip_zoh(rec):
    """Collapse the padding in place, recovering the real samples.

    Linear interpolation between real samples -- which is what LTspice does
    with a PWL source -- reconstructs the signal far better than the 5x
    staircase, which would inject spurious high-frequency content into a
    simulation. Returns (factor, confidence).
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


def trim_to_loop(rec, channel=None):
    """Trim the record to a whole number of fundamental cycles.

    Tiling a record that does not start and end at the same point in its cycle
    injects a step discontinuity at every wrap -- a transient that is not in
    the real signal but that a simulated circuit will respond to. Trimming to
    an integer number of periods, matched in both value and slope, makes
    repeated tiling produce a genuinely continuous stimulus.

    Returns (ok, message). Only meaningful for sustained, quasi-periodic
    signals; a decaying transient has no period to align to.
    """
    nums = sorted(rec.channels)
    ref = rec.channels[channel if channel in rec.channels else nums[0]]
    v = ref.volts()
    n = len(v)
    if n < 64:
        return False, "record too short to loop-align"

    m = measure(v, rec.dt)
    if m["freq"] <= 0:
        return False, "no periodicity found; leaving the record untrimmed"
    period = 1.0 / (m["freq"] * rec.dt)          # fundamental period, in samples
    if not np.isfinite(period) or period < 4 or period > n / 2.0:
        return False, ("fundamental spans %.3g samples of %d; need at least two "
                       "cycles to loop-align" % (period, n))

    z = v - v.mean()
    d = np.gradient(z)
    vscale = float(np.abs(z).max()) or 1.0
    dscale = float(np.abs(d).max()) or 1.0

    # Candidate start points: upward zero crossings, plus the record start.
    # With harmonics there can be several crossings per cycle and the first is
    # not necessarily the best place to cut, so score them all -- but only
    # within the first period, since starting later just discards a whole cycle
    # for no gain (every phase is already reachable inside one period).
    ups = [int(u) for u in np.nonzero((z[:-1] <= 0) & (z[1:] > 0))[0] if u < period]
    starts = [0] + ups[:8]

    best = None
    for start in starts:
        cycles = int((n - 1 - start) // period)
        if cycles < 1:
            continue
        target = start + cycles * period
        lo = max(start + 1, int(target - period / 4.0))
        hi = min(n, int(target + period / 4.0) + 1)
        if hi <= lo:
            continue
        idx = np.arange(lo, hi)
        # Sample `end` is the first one not included, so for a clean periodic
        # extension it should look like sample `start` in value and in slope.
        cost = ((z[idx] - z[start]) / vscale) ** 2 + ((d[idx] - d[start]) / dscale) ** 2
        j = int(np.argmin(cost))
        if best is None or cost[j] < best[0]:
            best = (float(cost[j]), start, int(idx[j]), cycles)

    if best is None:
        return False, "could not bracket a loop end point"
    _, start, end, cycles = best

    for ch in rec.channels.values():
        ch.raw = ch.raw[start:end].copy()

    # Report the wrap against the local sample-to-sample step: a wrap on a
    # steep part of the waveform is *supposed* to step. A ratio near or below
    # 1 means the seam is indistinguishable from an ordinary sample.
    seam = abs(float(v[start]) - float(v[end - 1]))
    local = abs(float(v[start + 1]) - float(v[start])) if start + 1 < n else 0.0
    ratio = (seam / local) if local > 1e-12 else float("inf")
    return True, ("trimmed to %d cycle(s), %d points, %.6g s; wrap step %.4g V "
                  "(%.1fx the local sample step)"
                  % (cycles, end - start, (end - start) * rec.dt, seam, ratio))
