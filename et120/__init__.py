"""Read raw waveform data from an ET120MC2 / ET120 handheld oscilloscope.

The vendor Windows application is normally the only thing that talks to these
instruments. This package talks to one directly, over its USB serial port, and
exports captures in formats LTspice can use.

    from et120 import Scope, resolve_port, strip_zoh, export

    with Scope(resolve_port()) as scope:          # releases on the way out
        rec = scope.acquire()                     # a validated 2048-pt record
        strip_zoh(rec)                            # -> the ~410 real samples
        export(rec, "signal", want_pwl=True)

See PROTOCOL.md for the wire format and README.md for what the hardware can
and cannot do.
"""

__version__ = "1.1"

LIMITATIONS = """\
LIMITATIONS -- what this instrument can and cannot give you
-----------------------------------------------------------
* 410 real sample points per capture. Always. The instrument transmits a
  2048-byte record but it is 410 real samples with each one repeated 5 times
  (zero-order hold), at every timebase from 5 ns/div to 50 ms/div. There is no
  deeper memory to ask for; this is the hardware limit.

* The timebase is the only trade-off you have:
      record length = 16.384 * secs_per_div      (the record is 16.384 div long)
      sample rate   = 25 / secs_per_div
  e.g.  5 ms/div -> 5 kSa/s over 82 ms      1 ms/div -> 25 kSa/s over 16.4 ms
       20 us/div -> 1.25 MSa/s over 328 us

* 8-bit vertical resolution over about 10 divisions, so one code is
  volts_per_div/25.5. Scale small signals up with the V/div control before
  capturing, not afterwards in software.

* Captures are snapshots, not a continuous stream. Each takes ~1.3 s of wall
  time (re-arm, wait for the buffer to fill, transfer 4.3 kB at 115200 baud).
  Consecutive captures are NOT contiguous -- the instrument re-triggers in
  between -- so they must not be concatenated into one signal.

* Nothing can be configured over serial. Volts/div, timebase, trigger and
  coupling are all front-panel only; this package reads whatever they are set
  to and adapts.

* The front panel stops responding while the host is issuing commands, and
  comes back by itself shortly after the traffic stops. Waveforms saved on the
  instrument can be recalled by slot, which is the practical way to capture a
  one-shot event you cannot time against the capture cycle.

* At timebases faster than the ADC's real-time rate the instrument must be
  using equivalent-time sampling, so those records are only meaningful for
  repetitive signals. The nominal time axis still applies.

* A sound card records continuously at 48 kSa/s, but only from sources it can
  load: its ~10 kOhm input high-passes a capacitive source of a few nF at
  1/(2*pi*R*C) ~ 8 kHz, so low-frequency content arrives tens of dB down. A
  x10 scope probe presents 10 MOhm and moves that corner three decades lower,
  which is why this instrument is the right front end for high-impedance
  sources despite the short records.
"""

from .protocol import (                                              # noqa: F401
    CMD_DEEP, CMD_LOAD_STORED, CMD_PING, CMD_SCREEN,
    PKT_DEEP, PKT_HELLO, PKT_SCREEN, PKT_STORED,
    SECS_PER_DIV, SECS_PER_DIV_S, VOLTS_PER_DIV, STORED_SLOT_MAX,
    DEEP_ZERO_CODE, DEEP_COUNTS_PER_DIV, DEEP_SAMPLES_PER_DIV,
    SCREEN_COUNTS_PER_DIV, SCREEN_COLUMNS_PER_DIV,
    build_command, iter_packets,
)
from .decode import (                                                # noqa: F401
    Channel, Record, decode_packet, measure, validate,
)
from .process import detect_zoh, strip_zoh, trim_to_loop             # noqa: F401
from .transport import Scope, list_ports, resolve_port               # noqa: F401
from .export import (                                                # noqa: F401
    channel_series, describe, export, plot_record,
    write_csv, write_ltspice_raw, write_pwl,
)

__all__ = [
    "__version__", "LIMITATIONS",
    "SECS_PER_DIV", "SECS_PER_DIV_S", "VOLTS_PER_DIV", "STORED_SLOT_MAX",
    "DEEP_ZERO_CODE", "DEEP_COUNTS_PER_DIV", "DEEP_SAMPLES_PER_DIV",
    "SCREEN_COUNTS_PER_DIV", "SCREEN_COLUMNS_PER_DIV",
    "Scope", "resolve_port", "list_ports",
    "Channel", "Record", "decode_packet", "validate", "measure",
    "detect_zoh", "strip_zoh", "trim_to_loop",
    "describe", "export", "plot_record", "channel_series",
    "write_pwl", "write_csv", "write_ltspice_raw",
    "build_command", "iter_packets",
]
