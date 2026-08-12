# ET120MC2 Oscilloscope dump

<img width="770" height="970" alt="PXL_20260811_054342258_preview" src="https://github.com/user-attachments/assets/38ccbe2f-b096-47f3-976e-949aa683f166" />

Dump raw waveform data from an **ET120MC2 / ET120** handheld oscilloscope over
its USB serial port, and export it in formats LTspice can use.

The vendor Windows application is the only thing that normally talks to these
scopes. It is closed-source, locale-fragile, and contains a date-based kill
switch (see [Why bother](#why-bother)). This script talks to the instrument
directly, so you can get the actual samples out and do something useful with
them.

The wire protocol is documented in [PROTOCOL.md](PROTOCOL.md), reverse
engineered from the decompiled vendor application and verified against
hardware.

## Install

```
python -m pip install pyserial numpy
```

## Use

```
python scope_et120.py ports                    # find the scope
python scope_et120.py info --plot              # connect, report and plot one acquisition
python scope_et120.py capture -o signal        # -> signal.pwl  (for LTspice)
python scope_et120.py capture -o signal --all  # -> .pwl + .csv + .raw
python scope_et120.py capture -n 10 -o run     # 10 separate records
python scope_et120.py stored                   # what is saved on the instrument
python scope_et120.py stored 4 -o saved --all  # fetch slot 4
python scope_et120.py unfreeze                 # hand a stuck scope back to its panel
python scope_et120.py limits                   # what the hardware can/cannot do
python scope_et120.py sniff -o dump.bin        # log raw serial traffic
python scope_et120.py decode dump.bin -o sig   # decode a dump offline
```

`python -m et120` works too. The port is found automatically by USB vendor ID;
override with `-p COM7` or `-p /dev/ttyACM0` if you have more than one, and
`ports` lists what is attached.

### Capturing a one-shot event

You cannot time a one-shot event — a switching transient, a fault, a single
mechanical impulse — against a capture cycle that takes ~1.3 s. Left to itself
the tool re-arms the instrument before every capture, so it would almost always
catch the quiet bit.

**Use the instrument's own trigger, and collect afterwards.** In this order:

1. Set the trigger to **Single** (or Normal), level just above the noise floor.
2. Cause the event. The instrument captures it and holds it.
3. Collect it:

   ```
   python scope_et120.py capture --hold -o event --all --plot event.png
   ```

`--hold` reads the held buffer as-is instead of re-arming, so the triggered
acquisition is not thrown away. It is a single request — no polling — which is
also the pattern least likely to upset the instrument.

Do not change volts/div between triggering and collecting: the scaling comes
from the instrument's current settings, and the tool will warn you if the held
samples look inconsistent with them.

There is deliberately no "wait for the trigger" option. Detecting a trigger
would mean polling, and neither available command can be polled safely:
command 4 locks the instrument (see Limitations) and command 3 re-arms it,
consuming the Single-shot you are waiting for. Trigger by hand, then collect.

Two fallbacks:

* **Save it on the instrument and retrieve it later** with `stored` (see
  above). Useful when you want several events banked before going near a
  computer. The cost is that saved waveforms are screen records rather than
  deep records — 300 columns of min/max, clipped to the display — so they
  carry less information.

* **`capture --best-of N`** takes N acquisitions and keeps the largest
  peak-to-peak, and warns if the instrument never re-triggered. Only worth it
  when the event repeats and you can keep provoking it; the trigger is better.

`info` output looks like this:

```
connected on COM5
  deep record at 5ms/div
  410 points over 0.082 s   dt = 0.0002 s   5000 Sa/s   [5x padding removed]
  CH1  1 V/div (probe x10)
       decoded  Vpp 3.216     +Vp 3.216     -Vp 0         Vrms 1.977     f 99.91 Hz
       scope    Vpp 3.215     +Vp 3.215     -Vp 0         Vrms 1.96      f 100 Hz
```

The `decoded` line is computed from the raw samples by this script; the `scope`
line is what the instrument reports about the same acquisition. They should
agree — that is the built-in sanity check, and the script refuses any packet
whose decoded Vpp is more than 20 % from the instrument's own reading.

## Feeding the capture into LTspice

**To drive a circuit**, use the `.pwl` file. Add a voltage source and set its
value to:

```
PWL file=C:\path\to\signal.pwl
```

Then run a `.tran` analysis at least as long as the record (the header comment
at the top of the `.pwl` tells you its length). `--repeat N` tiles the record N
times if you want a longer run out of one capture.

**To just look at the capture**, open the `.raw` file in the LTspice waveform
viewer. It is an LTspice ASCII transient file. Use the `.pwl` for driving,
the `.raw` for viewing.

Output always uses `.` as the decimal separator regardless of your system
locale.

### Making a capture worth simulating

A sine from a signal generator will make almost any circuit look fine. If you
are simulating in order to reproduce a *problem*, the capture has to carry the
properties that cause it — usually crest factor and low-frequency content, not
bandwidth.

1. **Set V/div for the peak, not the average.** Scale so the loudest transient
   fills 6–8 divisions without touching the edge. With only 8 bits the quiet
   parts will be coarse; accept that. A clipped capture is worthless, a noisy
   tail is not.

2. **Set the timebase for the event you care about.** Attack transients need
   rate, sustained tone needs length — and you cannot have both, so take two
   captures. See the table above.

3. **Use Normal trigger, not Auto**, with the level just above the noise floor,
   so the record holds the event rather than whatever was on screen when the
   script asked.

4. **Take several and keep the best**, since you cannot time a one-shot event
   against a ~1.3 s capture cycle:

   ```
   python scope_et120.py capture -o event --best-of 10 --remove-dc --all
   ```

   `--best-of N` keeps the acquisition with the largest peak-to-peak and warns
   if all N came back identical, which means the scope never re-triggered.
   `--remove-dc` centres the result on 0 V.

5. **Check it before you simulate it.** `--plot` draws the capture, with the
   ADC's full-scale limits marked and the fraction of that range actually in
   use. Bare `--plot` opens a window, `--plot FILE` writes a PNG. With 8 bits
   there is not much resolution to spare, so anything much under half the
   range is worth re-scaling and re-taking. Needs `matplotlib`, which is
   otherwise not required.

6. **Match the simulation's timestep to the capture.** The PWL header comment
   gives `dt` and total length; run `.tran 0 <length> 0 <dt/2>` so LTspice
   does not step over your samples.

7. **If you need a longer stimulus than one record**, `--loop` trims the
   capture to a whole number of cycles, matched in value and slope, so
   `--repeat N` tiles it into a continuous signal instead of stepping at every
   wrap. Tiling an unaligned record injects a step discontinuity that the
   simulated circuit will respond to and that is not in the real signal. Only
   meaningful for sustained, quasi-periodic captures — a decaying transient
   has no period to align to, and the tool will say so and leave it alone.

### Model the source, not just the signal

A PWL source in LTspice has zero output impedance — it will drive any load
without sagging. Real signal sources will not. If the source you probed has a
significant output impedance, put it in the model explicitly, in series with
the PWL source:

```
V1  src 0   PWL file=event.pwl
Rs  src in  {source_resistance}     ; or a series C for a capacitive source
```

Otherwise a stage whose input impedance is too low will look fine in
simulation and misbehave on the bench, and no amount of capture fidelity will
show it. This matters most for high-impedance sources — sensor elements,
high-value dividers, anything capacitive — where the source impedance can be
comparable to or larger than the input impedance it is feeding.

## Worked examples

### Drive an LTspice circuit with a real signal

Probe the signal, set the V/div so it fills most of the screen, then:

```
python scope_et120.py capture -o input --remove-dc --plot input.png
```

Check `input.png` looks like the signal you meant to capture and that the
report says a decent fraction of the ADC range is in use. Then in LTspice, add
a voltage source with value `PWL file=C:\path\to\input.pwl` and run a `.tran`
at least as long as the record. The `.pwl` header comment tells you its length
and timestep.

### Capture something you cannot time by hand

Trigger and save the event on the instrument itself, then collect it later:

```
python scope_et120.py stored                          # 5 saved waveform(s) found.
python scope_et120.py stored 4 -o event --all --plot
```

### Make a long stimulus out of one short record

One record is at most 819 ms and usually far less. For a steady signal, trim
it to whole cycles and tile it:

```
python scope_et120.py capture -o tone --loop --repeat 50 --remove-dc
```

`--loop` reports what it did: `trimmed to 8 cycle(s), 400 points, 0.08 s; wrap
step 0.196 V (1.0x the local sample step)`. A ratio near 1 means the seam is
indistinguishable from an ordinary sample. Without `--loop` the tiling steps
at every wrap, which the simulated circuit will respond to.

### Compare a circuit's input and output

Probe both channels, then export each separately:

```
python scope_et120.py capture -o both --all          # both channels
python scope_et120.py capture -o in  -c 1            # CH1 only
```

The `.raw` file carries both channels and opens directly in the LTspice
waveform viewer, so you can put the measurement next to the simulation.

### Is my capture any good?

```
python scope_et120.py info --plot
```

The report prints the tool's own measurements next to the instrument's for the
same acquisition. If they disagree, something is wrong — that check is also
applied automatically and captures failing it by more than 20 % are rejected.
It also flags aliasing, clipping, and using too little of the ADC range.

### Work on a capture later, or on another machine

```
python scope_et120.py capture -o run --dump      # also writes run.bin
python scope_et120.py decode run.bin -o run --all --plot run.png
```

`decode` needs no hardware, so raw dumps can be archived or sent to someone
else and re-analysed with different options.

## Limitations

Read these before planning anything around this instrument.

| | |
|---|---|
| Points per capture | **410 real samples. Always.** |
| Record length | `16.384 × secs_per_div` |
| Sample rate | `25 / secs_per_div` |
| Vertical resolution | 8 bits over ~10 divisions (`volts_per_div / 25.5` per code) |
| Time per capture | ~1.3 s wall clock |
| Continuous streaming | not possible |

* **The 2048-byte record is not 2048 samples.** The scope acquires 410 real
  samples and repeats each one 5 times to fill the buffer. This holds at every
  timebase from 5 ns/div to 50 ms/div — verified by sweeping the full range.
  The script detects the padding and collapses it by default, so you get the
  410 real points at the true sample interval. Linear interpolation between
  real points (what LTspice does with a PWL source) reconstructs the signal
  much better than the 5× staircase, which would inject spurious
  high-frequency content into your simulation. Use `--keep-zoh` to keep all
  2048 anyway.

* **The timebase is your only trade-off** between record length and sample
  rate, since the point count is fixed:

  | Timebase | Sample rate | Record length | Usable bandwidth |
  |---|---|---|---|
  | 20 µs/div | 1.25 MSa/s | 328 µs | 625 kHz |
  | 100 µs/div | 250 kSa/s | 1.64 ms | 125 kHz |
  | 1 ms/div | 25 kSa/s | 16.4 ms | 12.5 kHz |
  | 5 ms/div | 5 kSa/s | 82 ms | 2.5 kHz |
  | 50 ms/div | 500 Sa/s | 819 ms | 250 Hz |

* **Captures are snapshots, not a stream.** Consecutive captures are *not*
  contiguous in time — the scope re-triggers in between — so they must not be
  concatenated into one signal. `capture -n` writes them as separate files for
  that reason.

* **Nothing can be configured over serial.** Volts/div, timebase, trigger and
  coupling are front-panel only — there is no command for any of them, and the
  vendor application has no controls for them either. The tool reads whatever
  the instrument is set to and adapts. See PROTOCOL.md for how that was
  established.

* **The front panel goes unresponsive while the host is talking to it**, and
  normally comes back by itself a moment after the traffic stops. That is the
  instrument's behaviour, not a bug in this tool; the vendor application does
  it too. Polling it to "make sure" it is released makes things worse, since
  every extra command re-freezes it. The tool leaves it in live mode and then
  stops talking, and `Scope` is a context manager so even a crash cannot leave
  it held.

* **The firmware can hang, and it gets hot.** The front panel goes completely
  dead, the instrument becomes noticeably warm, and the serial side carries on
  answering normally the whole time. The warmth is the tell: it is not a
  protocol state you can command your way out of, it is the firmware spinning.
  The serial interface keeps replying because that runs from an interrupt
  while the main loop is starved.

  > **Unplug the USB cable promptly** — it is what is keeping the instrument
  > powered and heating. Then power off, power on, and reconnect USB.

  Pressing the power button *alone does nothing*: the instrument is
  bus-powered, so with USB connected it never actually loses power and comes
  back still hung. Front-panel settings return to defaults after a genuine
  cold boot, so re-set volts/div, timebase and trigger mode.

  No serial command recovers this state — `unfreeze` included; it is worth one
  try for the milder lock above, but do not keep poking a hot instrument.

  **The cause is not pinned down, but command pacing is strongly implicated.**
  The vendor application polls once a second and reads continuously in
  between, and never disturbs the instrument. Reproducing exactly that cadence
  from Python — one command per second, port held open — ran for 90 seconds
  with no hang. Issuing commands back to back does hang it, sometimes after
  only a handful.

  So commands are paced at one per second by default, matching the vendor.
  That is why a capture takes seconds rather than milliseconds. `--interval`
  changes it; lowering it is faster and has been seen to hang the instrument.

  (The port configuration also matches the vendor application exactly now,
  DTR and RTS held low included, which pyserial otherwise raises. That turned
  out not to be the cause, but there is no reason to differ.)

  Long runs of deep-record (command 4) requests with no command 3 between them
  are a separate and better-established hazard, so nothing here polls that way
  and the tool warns if anything issues 20 in a row. If a hang would be costly,
  `--screen` uses only command 3.

* **Fast timebases use equivalent-time sampling.** No handheld scope samples at
  5 GSa/s. Above the ADC's real-time rate the instrument builds the record from
  many triggers, so those captures are only meaningful for *repetitive*
  signals. The nominal time axis still applies.

* **Aliasing is your problem.** The instrument will happily hand you a record
  of a 30 kHz signal at 5 ms/div, aliased down to 1.4 kHz, and its own
  frequency counter will report nonsense too. The script warns when the
  dominant frequency exceeds 40 % of Nyquist.

* **Records are short, so plan around snapshots.** 410 points is not much if
  you were hoping to capture seconds of a signal.

  For audio-rate work a sound card is the obvious alternative — 48 kSa/s at
  16 bits, continuously — but only for sources it can load. Its input is
  typically ~10 kΩ, which is far too low for a high-impedance source: a
  capacitive source of a few nF feeding 10 kΩ is high-passed at
  `1 / (2πRC)` ≈ 8 kHz, so anything low-frequency arrives tens of dB down and
  you record essentially nothing. A ×10 scope probe presents 10 MΩ and moves
  that corner three decades lower. For such sources this instrument is the
  right front end despite the short records; a ×1 probe (usually 1 MΩ) is not
  necessarily enough.

## Why bother

The decompiled vendor application (`ScopeMeterDecompiled/`) contains:

```csharp
private bool bValidFile()
{
    DateTime dt = DateTime.Now;
    return dt.Year * 365 + dt.Month * 30 < 739550;
}
```

`2026 × 365 = 739490`, so this returns false once `Month` reaches 2 — i.e. from
**February 2026 onwards, permanently**. In `onDraw()` a false result pins the
trace-drawing loop's start index to 1, which skips channel index 0. The effect
is that **CH1 silently stops being drawn** and the display looks dead, while
everything else in the application carries on as if fine.

It is a deliberate kill switch, and a nasty one — it fails silently and looks
like broken hardware rather than expired software.

Separately, several `float.Parse` / `int.Parse` calls run under the current
culture, so on a comma-decimal locale the application misparses its own
calibration constants. Forcing an invariant culture at startup fixes that part.

### Getting the vendor application working again

Both issues are fixed in the sources under `ScopeMeterDecompiled/`, but that
tree does not rebuild cleanly — it is decompiler output, not the original
project.

So the practical fix is to patch the shipped binary instead. That is what
`ScopeMeterPatchedWithoutKillSwitch.zip` contains: a complete vendor install
with `bValidFile()` patched out in [dnSpy](https://github.com/dnSpy/dnSpy) so
it always returns true. The untouched original is kept alongside it in the
archive as `ScopeMeter.exe.old` if you want to diff or revert.

Unzip it over a normal installation, or just run `ScopeMeter.exe` from it. CH1
draws again.

You do not need any of this to use `scope_et120.py` — the script supersedes the
application entirely for getting data out. The patched build is only useful if
you want the original UI back.

## Files

| | |
|---|---|
| `scope_et120.py` | launcher — equivalent to `python -m et120` |
| `et120/protocol.py` | wire format: constants, framing, lookup tables |
| `et120/decode.py` | packets to calibrated volts; validation |
| `et120/process.py` | zero-order-hold removal, loop alignment |
| `et120/transport.py` | serial port discovery and the `Scope` connection |
| `et120/export.py` | reporting, plotting, PWL/CSV/raw writers |
| `et120/cli.py` | command line |
| `PROTOCOL.md` | wire protocol, packet layouts, scaling constants |
| `ScopeMeterDecompiled/` | decompiled vendor sources, kill switch and locale bugs fixed (reference only — does not rebuild cleanly) |
| `ScopeMeterPatchedWithoutKillSwitch.zip` | vendor install with the kill switch patched out of the binary via dnSpy; original kept as `ScopeMeter.exe.old` |

## Using it as a library

```python
from et120 import Scope, resolve_port, strip_zoh, measure, export

with Scope(resolve_port()) as scope:      # released on the way out, even on error
    rec = scope.acquire()                 # a validated 2048-point record
    strip_zoh(rec)                        # -> the ~410 real samples
    ch = rec.channels[1]
    print(measure(ch.volts(), rec.dt))    # {'vpp': ..., 'vrms': ..., 'freq': ...}
    export(rec, "signal", want_pwl=True)
```

`Record.channels` maps 1 and 2 to `Channel` objects. `Channel.volts()` gives
calibrated volts, and `rec.dt` is the sample interval. Everything the
instrument reported about the same acquisition is on the channel too, as
`rep_vpp`, `rep_vrms`, `rep_freq` and friends — useful for cross-checking.

## Licence

MIT. Protocol documentation is the result of clean-room-ish analysis of a
decompiled binary for interoperability purposes; check your local rules before
redistributing the decompiled sources themselves.
