"""Command line interface."""

import argparse
import os
import sys
import time

from . import LIMITATIONS, __version__
from .decode import decode_packet, validate
from .export import describe, export, plot_record, timebase_label
from .process import strip_zoh, trim_to_loop
from .protocol import CMD_DEEP, PKT_DEEP, PKT_SCREEN, STORED_SLOT_MAX, iter_packets
from .transport import USB_VID, Scope, list_ports, resolve_port

PROG = "scope_et120"


# -- shared helpers ---------------------------------------------------------

def _connect(args):
    port = resolve_port(args.port)
    scope = Scope(port, verbose=args.verbose)
    mode = scope.ping()
    if mode is None:
        scope.close()
        sys.exit("no reply on %s.\n"
                 "Check that the instrument is powered on and connected, that this is "
                 "the right\nport (try: %s ports), and that the vendor application is "
                 "closed." % (port, PROG))
    if mode != 0:
        print("note: the instrument answered in multimeter (DMM) mode. Switch it to "
              "scope mode\n      to capture waveforms.", file=sys.stderr)
    return scope


def _prep(rec, args):
    """Apply the signal-conditioning options to a freshly decoded record."""
    if not getattr(args, "keep_zoh", False) and rec.deep:
        strip_zoh(rec)
    if getattr(args, "loop", False):
        ok, why = trim_to_loop(rec, getattr(args, "channel", None))
        print("  loop-align: %s" % why)
        if not ok and getattr(args, "repeat", 1) > 1:
            print("              --repeat will therefore step at each wrap.",
                  file=sys.stderr)
    elif getattr(args, "repeat", 1) > 1:
        print("  note: --repeat without --loop tiles the record as captured, so the "
              "waveform\n        steps at each wrap unless it happens to be "
              "period-aligned.", file=sys.stderr)
    return rec


def _wants(args):
    """Which output formats were asked for; PWL is the default."""
    return (args.all or args.pwl or not (args.csv or args.raw),
            args.all or args.csv,
            args.all or args.raw)


def _emit(rec, stem, args, plot_title=None):
    describe(rec)
    if stem:
        want_pwl, want_csv, want_raw = _wants(args)
        export(rec, stem, want_pwl, want_csv, want_raw,
               channel=args.channel, repeat=args.repeat,
               remove_dc=args.remove_dc, version=__version__)
    if args.plot:
        plot_record(rec, None if args.plot == "-" else args.plot,
                    channel=args.channel, remove_dc=args.remove_dc, title=plot_title)


# -- commands ---------------------------------------------------------------

def cmd_ports(args):
    found = list_ports()
    if not found:
        print("no serial ports found")
        return 1
    for p in found:
        note = "   <-- likely the instrument" if p.vid == USB_VID else ""
        print("%-14s %s%s" % (p.device, p.description or "", note))
    return 0


def cmd_info(args):
    scope = _connect(args)
    with scope:
        print("connected on %s" % scope.port_name)
        rec = _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
        describe(rec)
        if args.plot:
            plot_record(rec, None if args.plot == "-" else args.plot)
    return 0


def _acquire_best(scope, args, n):
    """Take n acquisitions and keep the one with the largest peak-to-peak.

    For one-shot events that cannot be timed by hand against a ~1.3 s capture
    cycle. Recalling a waveform saved on the instrument is usually better --
    see the 'stored' command.
    """
    if n <= 1:
        return _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
    best, best_vpp, seen = None, -1.0, set()
    for k in range(n):
        rec = _prep(scope.acquire(deep=not args.screen, settle=args.settle), args)
        vpp = max(ch.span_volts() for ch in rec.channels.values())
        seen.add(next(iter(rec.channels.values())).raw.tobytes())
        print("    candidate %d/%d: Vpp %.4g V" % (k + 1, n, vpp))
        if vpp > best_vpp:
            best, best_vpp = rec, vpp
    if len(seen) == 1:
        print("    WARNING: all %d candidates were identical. The instrument is "
              "probably not\n             re-triggering -- check the trigger mode and "
              "level." % n, file=sys.stderr)
    return best


def cmd_capture(args):
    scope = _connect(args)
    stem = os.path.splitext(args.output)[0] if args.output else "capture"
    with scope:
        for i in range(args.count):
            label = stem if args.count == 1 else "%s_%02d" % (stem, i)
            print("capture %d/%d ..." % (i + 1, args.count))
            rec = _acquire_best(scope, args, args.best_of)
            _emit(rec, label, args)
            if args.dump:
                with open("%s.bin" % label, "wb") as fh:
                    fh.write(scope.log)
                print("  wrote %s.bin (raw serial log)" % label)
                scope.log = bytearray()
    if args.count > 1:
        print("\nNote: these records are NOT contiguous in time -- the instrument "
              "re-triggers\n      between captures. Do not concatenate them into one "
              "signal.")
    return 0


def cmd_stored(args):
    """No slot given -> report what the instrument holds. Slot given -> fetch it."""
    scope = _connect(args)
    with scope:
        if args.slot is None:
            print("scanning slots 1-%d (pass a slot number to fetch one)..." % args.scan)
            found = 0
            for slot in range(1, args.scan + 1):
                rec = scope.fetch_stored(slot)
                if rec is None:
                    continue
                found += 1
                ch = rec.channels[min(rec.channels)]
                print("  slot %3d: %-7s/div  %-9.6g V/div  CH%s  Vpp %-9.4g f %.6g Hz"
                      % (slot, timebase_label(rec), ch.volts_per_div,
                         "+".join(str(c) for c in sorted(rec.channels)),
                         ch.rep_vpp, ch.rep_freq))
            print("%d saved waveform(s) found." % found if found
                  else "No saved waveforms found in slots 1-%d." % args.scan)
            return 0

        rec = scope.fetch_stored(args.slot)
        if rec is None:
            sys.exit("slot %d holds no waveform (or the instrument did not answer)."
                     % args.slot)
        _prep(rec, args)
        print("slot %d:" % args.slot)
        _emit(rec, os.path.splitext(args.output)[0] if args.output else None, args,
              plot_title="ET120MC2 stored slot %d" % args.slot)
    return 0


def cmd_decode(args):
    with open(args.dumpfile, "rb") as fh:
        data = fh.read()
    packets = [p for _, t, p in iter_packets(data)
               if t in (PKT_DEEP, PKT_SCREEN, 0x22)]
    if not packets:
        sys.exit("no waveform packets found in %s" % args.dumpfile)
    good = []
    for pkt in packets:
        rec = decode_packet(pkt)
        if rec is not None and validate(rec)[0]:
            good.append(rec)
    print("%s: %d waveform packet(s), %d valid"
          % (args.dumpfile, len(packets), len(good)))
    if not good:
        sys.exit("no packet passed validation (all were stale or incomplete buffers)")
    deep = [r for r in good if r.deep]
    chosen = _prep((deep or good)[0], args)
    _emit(chosen, os.path.splitext(args.output)[0] if args.output else None, args)
    return 0


def cmd_sniff(args):
    scope = _connect(args)
    print("logging raw traffic for %.1f s (polling with command %d) ..."
          % (args.seconds, args.cmd))
    with scope:
        end, nxt = time.time() + args.seconds, 0.0
        while time.time() < end:
            if time.time() >= nxt:
                scope.send(args.cmd)
                nxt = time.time() + args.interval
            chunk = scope.sp.read(65536)
            if chunk:
                scope.log += chunk
    with open(args.output, "wb") as fh:
        fh.write(scope.log)
    print("wrote %s (%d bytes)" % (args.output, len(scope.log)))
    for off, ptype, pkt in iter_packets(scope.log):
        print("  @%-8d type 0x%02x  %d bytes" % (off, ptype, len(pkt)))
    return 0


def cmd_limits(args):
    print(LIMITATIONS)
    return 0


# -- argument parsing -------------------------------------------------------

def _add_acq_opts(p):
    g = p.add_argument_group("acquisition")
    g.add_argument("--screen", action="store_true",
                   help="use the 600-point screen record instead of the "
                        "2048-point deep record")
    g.add_argument("--settle", type=float, default=0.25, metavar="SEC",
                   help="how long to let the deep buffer fill after re-arming")


def _add_signal_opts(p):
    g = p.add_argument_group("signal conditioning")
    g.add_argument("-c", "--channel", type=int, choices=(1, 2),
                   help="use only this channel")
    g.add_argument("--remove-dc", action="store_true",
                   help="subtract the mean, centring the waveform on 0 V")
    g.add_argument("--loop", action="store_true",
                   help="trim to a whole number of cycles so --repeat tiles without "
                        "a step at each wrap (sustained signals only)")
    g.add_argument("--keep-zoh", action="store_true",
                   help="keep the 5x sample padding instead of collapsing it")


def _add_output_opts(p):
    g = p.add_argument_group("output")
    g.add_argument("-o", "--output", metavar="STEM", help="output file stem")
    g.add_argument("--pwl", action="store_true",
                   help="write an LTspice PWL source file (the default)")
    g.add_argument("--csv", action="store_true", help="write CSV")
    g.add_argument("--raw", action="store_true", help="write an LTspice ASCII .raw")
    g.add_argument("--all", action="store_true", help="write all three formats")
    g.add_argument("--repeat", type=int, default=1, metavar="N",
                   help="tile the PWL N times for a longer transient run")
    g.add_argument("--plot", nargs="?", const="-", metavar="FILE",
                   help="plot it to check it; bare --plot opens a window, "
                        "--plot FILE writes a PNG (needs matplotlib)")


def _add_export_opts(p):
    _add_signal_opts(p)
    _add_output_opts(p)


def build_parser():
    ap = argparse.ArgumentParser(
        prog=PROG,
        description="Read raw waveform data from an ET120MC2 oscilloscope and "
                    "export it for LTspice.",
        epilog="Run '%s limits' for what the hardware can and cannot do." % PROG)
    ap.add_argument("-p", "--port", metavar="PORT",
                    help="serial port, e.g. COM5 or /dev/ttyACM0. Autodetected by "
                         "USB vendor ID if omitted.")
    ap.add_argument("-v", "--verbose", action="store_true")
    ap.add_argument("--version", action="version", version="%s %s" % (PROG, __version__))
    sub = ap.add_subparsers(dest="cmd")

    p = sub.add_parser("ports", help="list serial ports")
    p.set_defaults(func=cmd_ports)

    p = sub.add_parser("info", help="connect and report one acquisition")
    _add_acq_opts(p)
    p.add_argument("--keep-zoh", action="store_true")
    p.add_argument("--plot", nargs="?", const="-", metavar="FILE",
                   help="plot the acquisition; bare --plot opens a window, "
                        "--plot FILE writes a PNG (needs matplotlib)")
    p.set_defaults(func=cmd_info)

    p = sub.add_parser("capture", help="capture waveform(s) and export")
    p.add_argument("-n", "--count", type=int, default=1, metavar="N",
                   help="number of separate records to take")
    p.add_argument("--best-of", type=int, default=1, metavar="N",
                   help="take N acquisitions per record and keep the largest "
                        "peak-to-peak, for one-shot events")
    p.add_argument("--dump", action="store_true", help="also save the raw serial log")
    _add_acq_opts(p)
    _add_export_opts(p)
    p.set_defaults(func=cmd_capture)

    p = sub.add_parser("stored",
                       help="list waveforms saved on the instrument, or fetch one")
    p.add_argument("slot", nargs="?", type=int,
                   help="slot to fetch; omit to see what the instrument holds")
    p.add_argument("--scan", type=int, default=20, metavar="N",
                   help="how many slots to look through when listing "
                        "(default 20, the instrument allows up to %d)" % STORED_SLOT_MAX)
    _add_export_opts(p)
    p.set_defaults(func=cmd_stored)

    p = sub.add_parser("decode", help="decode a saved raw serial dump offline")
    p.add_argument("dumpfile")
    _add_export_opts(p)
    p.add_argument("--screen", action="store_true", help=argparse.SUPPRESS)
    p.set_defaults(func=cmd_decode)

    p = sub.add_parser("sniff", help="log raw serial traffic to a file")
    p.add_argument("-o", "--output", default="dump.bin", metavar="FILE")
    p.add_argument("--cmd", type=int, default=CMD_DEEP, metavar="N",
                   help="command byte to poll with")
    p.add_argument("--interval", type=float, default=1.0, metavar="SEC")
    p.add_argument("--seconds", type=float, default=6.0, metavar="SEC")
    p.set_defaults(func=cmd_sniff)

    p = sub.add_parser("limits", help="print what the hardware can and cannot do")
    p.set_defaults(func=cmd_limits)

    return ap


def main(argv=None):
    ap = build_parser()
    args = ap.parse_args(argv)
    if not getattr(args, "cmd", None):
        ap.print_help()
        return 1
    try:
        return args.func(args)
    except KeyboardInterrupt:
        print("\ninterrupted", file=sys.stderr)
        return 130
