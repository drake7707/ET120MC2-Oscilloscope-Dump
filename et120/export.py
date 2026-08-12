"""Reporting, plotting and file output."""

import sys
import time

try:
    import numpy as np
except ImportError:
    sys.exit("numpy is required:  python -m pip install numpy")

from .decode import measure
from .protocol import DEEP_COUNTS_PER_DIV, DEEP_ZERO_CODE, SECS_PER_DIV_S


def timebase_label(rec):
    return (SECS_PER_DIV_S[rec.timebase_index]
            if rec.timebase_index < len(SECS_PER_DIV_S) else "?")


def describe(rec, stream=sys.stdout):
    """Print a human-readable summary, cross-checked against the instrument."""
    kind = "deep record" if rec.deep else "screen record (min/max peak detect)"
    if rec.slot is not None:
        kind += " from slot %d" % rec.slot
    print("  %s at %s/div" % (kind, timebase_label(rec)), file=stream)
    print("  %d points over %.6g s   dt = %.6g s   %.6g Sa/s%s"
          % (rec.npoints, rec.duration, rec.dt, 1.0 / rec.dt if rec.dt else 0.0,
             "" if rec.zoh <= 1 else "   [%dx padding removed]" % rec.zoh),
          file=stream)
    for num in sorted(rec.channels):
        ch = rec.channels[num]
        m = measure(ch.volts(), rec.dt)
        used = ch.range_used()
        print("  CH%d  %.6g V/div (probe x%d)   using %.0f%% of the ADC range"
              % (num, ch.volts_per_div, 10 ** ch.probe_exp, used * 100), file=stream)
        print("       decoded  Vpp %-9.4g +Vp %-9.4g -Vp %-9.4g Vrms %-9.4g f %.6g Hz"
              % (m["vpp"], m["vmax"], m["vmin"], m["vrms"], m["freq"]), file=stream)
        print("       scope    Vpp %-9.4g +Vp %-9.4g -Vp %-9.4g Vrms %-9.4g f %.6g Hz"
              % (ch.rep_vpp, ch.rep_vp_pos, ch.rep_vp_neg, ch.rep_vrms, ch.rep_freq),
              file=stream)
        if used < 0.25:
            print("       NOTE: only %.0f%% of the ADC range is in use -- with 8 bits "
                  "that is\n             coarse. Turn up the V/div sensitivity if you "
                  "can." % (used * 100), file=stream)
        if ch.raw.min() == 0 or ch.raw.max() == 255:
            print("       WARNING: samples reach the end of the ADC range; the capture "
                  "may be clipped.", file=stream)
        if ch.clipped:
            print("       WARNING: trace is clipped at the edge of the display; "
                  "voltages are wrong.", file=stream)
        if not rec.deep:
            print("       NOTE: screen record -- the zero reference is inferred from "
                  "the scope's -Vp.", file=stream)
        nyq = 0.5 / rec.dt if rec.dt else 0.0
        if m["freq"] > 0.4 * nyq > 0:
            print("       WARNING: %.6g Hz is near Nyquist (%.6g Hz) -- the capture is "
                  "likely aliased.\n                Use a faster timebase."
                  % (m["freq"], nyq), file=stream)


def channel_series(rec, channel=None, remove_dc=False, quiet=True):
    """Return (t, [(name, volts), ...]) exactly as the exporters see it."""
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
    return t, named


# -- writers ----------------------------------------------------------------

def write_pwl(path, t, v, repeat=1):
    """LTspice piecewise-linear source file:  'V1 in 0 PWL file=<path>'.

    Always writes '.' as the decimal separator regardless of system locale, and
    '\\n' line endings regardless of platform.
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


def write_ltspice_raw(path, t, channels, title="ET120MC2 capture", version=""):
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
        fh.write("Command: scope_et120 %s\n" % version)
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
           channel=None, repeat=1, quiet=False, remove_dc=False, version=""):
    t, named = channel_series(rec, channel, remove_dc, quiet=quiet)
    written = []
    if want_pwl:
        for name, v in named:
            path = "%s.pwl" % stem if len(named) == 1 else "%s_%s.pwl" % (stem, name)
            written.append(write_pwl(path, t, v, repeat=repeat))
    if want_csv:
        written.append(write_csv("%s.csv" % stem, t, named))
    if want_raw:
        written.append(write_ltspice_raw("%s.raw" % stem, t, named, version=version))
    if not quiet:
        for p in written:
            print("  wrote %s" % p)
    return written


# -- plotting ---------------------------------------------------------------

def _time_unit(span):
    for factor, name in ((1.0, "s"), (1e3, "ms"), (1e6, "us"), (1e9, "ns")):
        if span * factor >= 1.0:
            return factor, name
    return 1e9, "ns"


def plot_record(rec, path=None, channel=None, remove_dc=False, title=None):
    """Show or save a plot of the capture, with the ADC's range marked.

    matplotlib is imported lazily so it stays an optional dependency.
    """
    try:
        import matplotlib
        if path:
            matplotlib.use("Agg")
        import matplotlib.pyplot as plt
    except ImportError:
        print("--plot needs matplotlib:  python -m pip install matplotlib",
              file=sys.stderr)
        return False

    t, named = channel_series(rec, channel, remove_dc)
    factor, unit = _time_unit(rec.duration)
    ts = t * factor

    fig, ax = plt.subplots(figsize=(11, 5))
    for (name, v), colour in zip(named, ("tab:red", "tab:olive")):
        num = int(name[2:])
        ch = rec.channels[num]
        m = measure(v, rec.dt)
        ax.plot(ts, v, colour, lw=1.0,
                label=("CH%d    Vpp %.4g    max %.4g    min %.4g\n"
                       "rms %.4g    avg %.4g    f %.6g Hz"
                       % (num, m["vpp"], m["vmax"], m["vmin"],
                          m["vrms"], m["vmean"], m["freq"])))
        # the mean, so any DC offset is visible against the trace
        ax.axhline(m["vmean"], color=colour, ls="-", lw=0.6, alpha=0.35)
        if rec.deep:
            # Mark the ADC's limits, shifted by whatever --remove-dc subtracted
            # so the headroom shown stays the real headroom.
            shift = float(ch.volts().mean()) if remove_dc else 0.0
            lo = (0.0 - DEEP_ZERO_CODE) / DEEP_COUNTS_PER_DIV * ch.volts_per_div - shift
            hi = (255.0 - DEEP_ZERO_CODE) / DEEP_COUNTS_PER_DIV * ch.volts_per_div - shift
            ax.axhline(lo, color=colour, ls=":", lw=0.8, alpha=0.5)
            ax.axhline(hi, color=colour, ls=":", lw=0.8, alpha=0.5)
            ax.text(ts[-1], hi, " CH%d ADC full scale (%.0f%% used) "
                    % (num, ch.range_used() * 100),
                    color=colour, fontsize=8, va="bottom", ha="right")

    default = "ET120MC2  %s  %s/div  %d pts  %.6g Sa/s" % (
        "deep" if rec.deep else "screen", timebase_label(rec), rec.npoints,
        1.0 / rec.dt if rec.dt else 0.0)
    ax.set_title(title or default)
    ax.set_xlabel("time (%s)" % unit)
    ax.set_ylabel("volts")
    ax.grid(True, alpha=0.3)
    ax.legend(loc="best", fontsize=8, framealpha=0.9, labelspacing=0.8)
    ax.margins(x=0)
    fig.tight_layout()

    if path:
        fig.savefig(path, dpi=120)
        plt.close(fig)
        print("  wrote %s" % path)
    else:
        try:
            plt.show()
        except Exception as exc:
            # e.g. no display on a headless machine
            print("could not open a plot window (%s).\n"
                  "Use --plot FILE to write a PNG instead." % exc, file=sys.stderr)
            return False
    return True
