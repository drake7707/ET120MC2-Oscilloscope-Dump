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
python scope_et120.py info                     # connect, report one acquisition
python scope_et120.py capture -o signal        # -> signal.pwl  (for LTspice)
python scope_et120.py capture -o signal --all  # -> .pwl + .csv + .raw
python scope_et120.py capture -n 10 -o run     # 10 separate records
python scope_et120.py limits                   # what the hardware can/cannot do
python scope_et120.py sniff -o dump.bin        # log raw serial traffic
python scope_et120.py decode dump.bin -o sig   # decode a dump offline
```

Default port is `COM5`; override with `-p COM7` / `-p /dev/ttyACM0`.

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
   python scope_et120.py capture -o pluck --best-of 10 --remove-dc --all
   ```

   `--best-of N` keeps the acquisition with the largest peak-to-peak and warns
   if all N came back identical, which means the scope never re-triggered.
   `--remove-dc` centres the result on 0 V, which is usually what you want
   feeding an amplifier input.

5. **Match the simulation's timestep to the capture.** The PWL header comment
   gives `dt` and total length; run `.tran 0 <length> 0 <dt/2>` so LTspice
   does not step over your samples.

### Model the source, not just the signal

A PWL source in LTspice has zero output impedance. It will drive anything.
If the real source cannot, the simulation will not reproduce the real
behaviour no matter how good the recording is.

For a piezo pickup, model it as the recorded voltage in series with the
element's own capacitance:

```
V1  src 0   PWL file=pluck.pwl
C1  src in  2.2n          ; measured pickup capacitance
Rlk in  0   1000Meg       ; leakage, optional
```

That series capacitance is the whole point: at 41 Hz, 2.2 nF is a source
impedance of about 1.8 MΩ. An input stage that looks fine driven by an ideal
source will lose most of the signal — and tilt the frequency response — when
fed through that. Many handheld meters, including this one in DMM mode, will
measure the pickup's capacitance for you.

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

* **Requesting the deep record freezes the scope's front panel.** That is the
  instrument's behaviour, not a bug in this script; the vendor application does
  it too. Sending it back to live mode unfreezes it, which this script does
  automatically on exit and between captures.

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

  A sound card is the obvious alternative — 48 kSa/s at 16 bits, continuously —
  but only for sources it can actually load. It is the wrong answer for a
  high-impedance source. A piezo pickup (e.g. a Realist on a double bass) is
  effectively a capacitive source of a few nF, and a line input of ~10 kΩ forms
  a high-pass with it at `1 / (2πRC)` ≈ 8 kHz, putting a 41 Hz low E some 46 dB
  down. You record essentially nothing. A ×10 scope probe presents 10 MΩ, which
  moves that corner to ≈ 8 Hz and passes the whole instrument range — so for
  sources like this the scope really is the right front end, short records and
  all.

  (If you do want continuous recording from such a source, the missing piece is
  a high-impedance buffer ahead of the sound card — which is likely the very
  circuit you are simulating.)

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
| `scope_et120.py` | the tool |
| `PROTOCOL.md` | wire protocol, packet layouts, scaling constants |
| `ScopeMeterDecompiled/` | decompiled vendor sources, kill switch and locale bugs fixed (reference only — does not rebuild cleanly) |
| `ScopeMeterPatchedWithoutKillSwitch.zip` | vendor install with the kill switch patched out of the binary via dnSpy; original kept as `ScopeMeter.exe.old` |

## Licence

MIT. Protocol documentation is the result of clean-room-ish analysis of a
decompiled binary for interoperability purposes; check your local rules before
redistributing the decompiled sources themselves.
