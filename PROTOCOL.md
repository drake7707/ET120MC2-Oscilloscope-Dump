# ET120MC2 serial protocol

Reverse engineered from the decompiled vendor application
(`ScopeMeterDecompiled/ScopeMeter/Form1ShiBoQi.cs`) and verified against live
hardware.

## Link

| | |
|---|---|
| Transport | USB CDC virtual COM port (`VID_28E9` / `PID_018B`, a GD32/Geehy MCU) |
| Baud | 115200, 8N1, no flow control |
| Byte order | little endian |

The port is exclusive — the vendor application must be closed before anything
else can open it.

## Framing

Two frame shapes. Both start with the sync byte `0xA5` and are validated by
8-bit two's-complement checksums (every covered byte sums to 0 mod 256).

### Short frame — host commands and the `0x21` reply

```
A5  TYPE  PARAM  CHK          CHK = -(A5 + TYPE + PARAM)
```

### Long frame — all data packets

```
A5  TYPE  LEN_LO  LEN_HI  HCHK   <LEN payload bytes>   DCHK

HCHK = -(A5 + TYPE + LEN_LO + LEN_HI)                 header checksum
DCHK = -(A5 + TYPE + LEN_LO + LEN_HI + HCHK + sum(payload))
```

`LEN` is capped at 5000 by the vendor application. Note the receiver
resynchronises by scanning for `0xA5`, so a checksum failure just means
"keep looking".

## Commands (host → scope)

Sent as short frames with `PARAM = 0` unless noted.

| Cmd | Meaning | Reply |
|---:|---|---|
| 1 | Ping / identify | `0x21` |
| 2 | Load stored waveform, `PARAM` = slot | `0x22` |
| 3 | Live screen waveform | `0x23` |
| 4 | Deep record for analysis | `0x24` |
| 5 | Multimeter live reading | `0x25` |
| 6 | Multimeter auto-record | `0x26` |
| 7 | Multimeter hold record | `0x27` |
| 8 | Multimeter calibration | `0x28` |
| 9 | Product info | `0x29` |

Example — ping: `A5 01 00 5A`. Deep record: `A5 04 00 57`.

### There is no command to change instrument settings

Every command is a request for data or a switch of which data stream is being
sent. Nothing sets volts/div, timebase, trigger level, coupling or channel —
the instrument is effectively read-only over serial, which is also why the
vendor application has no controls for any of those.

This was checked two ways:

* Every send site in the vendor application was enumerated. `sendCommand` is
  called with 1, 3, 4, 5, 6 and 9; `sendBytes` with `{3}`, `{4}`, `{5}`, `{6}`,
  `{7}`, `{8}`, `{9}` and `{2, slot}`. `sendString` is never called at all —
  it is dead code.
* Command bytes 10–31 were sent to the hardware with `PARAM = 0`, reading the
  instrument's settings back before and after each one. Every one produced
  **zero bytes in reply and no change of state**, so the firmware's dispatcher
  ignores unrecognised command codes rather than acting on them.

`scope_et120.py` therefore reads the timebase and volts/div out of each packet
and adapts to whatever the front panel is set to, rather than trying to
command it.

One loose end: `getData()` accepts packet type `0x3A` (58) alongside the
documented `0x22`–`0x29`, but the handler switch has no case for it, so such a
packet would be parsed and discarded. Nothing was observed to emit one.

## Packets (scope → host)

Offsets below are into `TYPE || payload`, i.e. index 0 is the type byte. This
matches the vendor code's `tempPackage` layout.

### `0x21` — hello

4-byte short frame. `PARAM` is `0` when the instrument is in scope (DSO) mode,
non-zero when it is in multimeter (DMM) mode.

### `0x22` — stored waveform, 1362 bytes total

Returned in response to command 2 with the slot number in `PARAM`. It is a
screen record behind a four-byte prefix:

| Offset | Size | Meaning |
|---:|---:|---|
| 0 | 1 | `0x22` |
| 1 | 1 | slot number, echoed back |
| 2 | 2 | `00 06`, constant in every capture seen |
| 4 | 1358 | a `0x23` screen record, laid out exactly as below |

The vendor application drops the first four bytes and parses the remainder as
a `0x23`, which is what `et120` does.

**The instrument must be live for this to work.** Sending command 2 while it
is not simply wedges the serial interface for a while — no reply, and
subsequent requests go unanswered until it recovers on its own. It is not
enough to send command 3 first; you have to actually see the `0x23` reply come
back before issuing the recall. `Scope.fetch_stored()` does that.

Recalling a saved waveform is the practical way to capture a one-shot event:
trigger and save it on the instrument by hand, then retrieve it over serial at
leisure. The cost is that stored waveforms are screen records, not deep
records — 300 columns of min/max, clipped to the display and offset by the
vertical position control — so they carry less information than a live `0x24`.

### `0x23` — live screen record, 1358 bytes total

| Offset | Size | Meaning |
|---:|---:|---|
| 0 | 1 | `0x23` |
| 1 | 1 | active-channel mask: 1 = CH1, 2 = CH2, 3 = both |
| 2 | 600 | CH1 samples |
| 602 | 600 | CH2 samples |
| 1202 | 156 | metadata block (below) |

The 600 bytes are **300 screen columns of (min, max) pairs** — peak-detect
data at 30 columns per division, already clipped to the display and offset by
the vertical-position control. Good for reproducing the screen, poor for
signal analysis.

### `0x24` — deep record, 4254 bytes total

| Offset | Size | Meaning |
|---:|---:|---|
| 0 | 1 | `0x24` |
| 1 | 1 | active-channel mask |
| 2 | 2048 | CH1 samples |
| 2050 | 2048 | CH2 samples |
| 4098 | 156 | metadata block (below) |

This is the useful one: unclipped, position-independent ADC codes. **This is
what `scope_et120.py` uses.**

### Metadata block (both packet types)

| Offset | Size | Meaning |
|---:|---:|---|
| 0 | 1 | channel enum (0 none, 1 CH1, 2 CH2, 3 both) |
| 1 | 1 | channel currently selected on the front panel |
| 2 | 1 | timebase index (see table below) |
| 10 | 73 | CH1 parameter block |
| 83 | 73 | CH2 parameter block |

Bytes 3–9 and everything past offset 156 were zero in every capture.

### Channel parameter block (73 bytes)

| Offset | Type | Meaning |
|---:|---|---|
| 0 | u8 | volts/div index (see table below) |
| 4 | u8 | probe attenuation exponent; volts/div is multiplied by 10^this |
| 25 | f32 | Vrms |
| 29 | f32 | Vpp |
| 37 | f32 | +Vpeak |
| 41 | f32 | −Vpeak |
| 45 | f32 | period, seconds |
| 49 | f32 | frequency, Hz |

The remaining bytes were constant or zero across captures and are unused by
the vendor application.

## Scaling

### Vertical

```
volts = (sample - 128) / 25.5 * volts_per_div
volts_per_div = VOLTS_PER_DIV[block[0]] * 10 ** block[4]
```

The 8-bit code range spans exactly 10 vertical divisions with 0 V at code 128,
so one division is 25.5 codes and one code is `volts_per_div / 25.5`.

This was established empirically, not taken from the vendor code (which only
ever uses the raw codes as screen coordinates). Validation on a 100 Hz,
0→3 V test sine at 1 V/div: codes spanned 128…210, giving a decoded Vpp of
3.2157 V against the instrument's own reported 3.2150 V, and a decoded −Vpeak
of exactly 0.0000 V. `scope_et120.py` re-runs this cross-check on every
capture and rejects packets that disagree by more than 20 %.

`VOLTS_PER_DIV` index table (`voltaBaseF` in the vendor source):

```
 0..9   10m 20m 50m 100m 200m 500m 1 2 5 10        (V/div)
10..19  100m 200m 500m 1 2 5 10 20 50 100
20..29  1 2 5 10 20 50 100 200 500 1000
```

### Horizontal

For the `0x24` deep record:

```
transmitted sample interval = secs_per_div / 125     (2048 pts = 16.384 div)
real sample interval        = secs_per_div / 25      (see ZOH note)
```

`SECS_PER_DIV` index table (`timeBaseF` in the vendor source):

```
 0  -      1  5ns    2  10ns   3  25ns   4  50ns   5  100ns  6  200ns  7  500ns
 8  1us    9  2us   10  5us   11  10us  12  20us  13  50us  14  100us 15  200us
16  500us 17  1ms   18  2ms   19  5ms   20  10ms  21  20ms  22  50ms  23  100ms
24  200ms 25  500ms 26  1s    27  2s    28  5s    29  10s   30  20s   31  50s
```

### The zero-order-hold padding

The 2048 transmitted samples are **not** 2048 independent measurements. The
scope acquires ~410 real samples (25 per division over 16.384 divisions) and
repeats each one 5 times to fill the buffer. On a captured 100 Hz sine every
sample appeared in a run of exactly 5 identical values, and only 82 distinct
code values occurred in 2048 samples.

`scope_et120.py` detects the repeat factor from the data and collapses it by
default, so you get the ~410 real points at the true sample interval. Linear
interpolation between real samples (what LTspice does with a PWL source) is a
much better reconstruction than feeding it the 5× staircase, which would
inject spurious high-frequency content. Use `--keep-zoh` to keep all 2048.

The factor was confirmed to be 5 with 100 % of sample groups constant at every
timebase reachable on the instrument — 5 ms, 1 ms, 20 µs, 10 µs, 5 µs, 2 µs,
1 µs, 500 ns, 25 ns and 5 ns per division. The point count is 410 in all
cases; there is no deeper memory to request.

### Verification of the time axis

* 100 Hz sine at 5 ms/div: the record's dominant period measured 250.22
  transmitted samples. At `5 ms / 125 = 40 µs` per sample that is 10.009 ms,
  i.e. 99.91 Hz against a nominal 100 Hz — 0.09 % error.
* 30 kHz sine at 20 µs/div and 10 µs/div: decoded 31.23 kHz and 31.18 kHz,
  against the instrument's own counter reading 32.05 kHz and 31.65 kHz.

Note that above the ADC's real-time sampling rate the instrument must be
composing the record from multiple triggers (equivalent-time sampling) — a
handheld scope does not sample at the 5 GSa/s that 5 ns/div nominally implies.
Fast-timebase records are therefore only meaningful for repetitive signals.

## Acquisition state machine — important

Requesting the deep record (cmd 4) puts the instrument into a **held remote
state**: it replays the identical buffer for every subsequent cmd 4, and the
front-panel controls stop responding. Repeated cmd 4 requests return
byte-identical data indefinitely.

Sending cmd 3 puts it back into live mode, which unfreezes the front panel and
re-arms acquisition. For the first few seconds after that, the deep buffer is
only partly filled and cmd 4 returns a **partly-zero garbage buffer** —
distinguishable by a tail of exact zeros and a repeat factor of 2 instead of 5.

So a fresh deep capture is:

```
cmd 3  ->  read/discard the 0x23 reply  ->  short settle  ->  cmd 4  ->  validate  ->  retry if bad
```

`et120` implements exactly this, validating each packet against the
instrument's own Vpp readout, and sends a final cmd 3 on exit so the scope is
left usable.

### The front panel while the host is talking

The instrument's front panel stops responding for as long as the host keeps
issuing commands, and comes back by itself a moment after the traffic stops.
This is not a latch that has to be cleared — polling it in a loop to "make
sure" it is released is actively counterproductive, because every extra
command re-freezes the panel and the last one leaves it freshly frozen.

So the correct release is: leave the instrument in live mode rather than
holding a deep record or a recalled waveform, read the reply so nothing is
half-sent, and then stop talking to it. `Scope` does this from `release()`, and
is a context manager so a crash cannot leave the instrument held.

Occasionally the lock does not clear on its own: the front panel stays dead
while the serial side keeps answering ping and command 3 normally. Recovery is
to **unplug USB, power off, power on, then reconnect USB** — a power cycle
alone does not work, because USB keeps the instrument alive and it returns
still locked. The front-panel settings are back at defaults afterwards.

The trigger has not been isolated. Issuing command 2 while the instrument is
not live reliably wedges the serial interface for a while, and sustained
back-to-back requests are the obvious suspect for the harder lock, but neither
has been shown to cause it.

## Notes on the vendor application

* Its FFT frequency axis uses a hard-coded `calibration_ratio_x = 15`, where
  the geometry implies 16.384. Its frequency readings are ~9 % high.
* `bValidFile()` is a date-based kill switch:

  ```csharp
  return dt.Year * 365 + dt.Month * 30 < 739550;
  ```

  `2026 * 365 = 739490`, so it goes false once `Month` reaches 2 — from
  **February 2026 onward, permanently**. In `onDraw()` a false result pins the
  trace-drawing loop's start index to 1, skipping channel index 0, so **CH1
  stops being drawn** while the rest of the application behaves normally. It
  fails silently and looks like broken hardware.
* The locale bug is real too — several `float.Parse` / `int.Parse` calls use
  the current culture, so a comma-decimal locale misparses the calibration
  constants read back from the registry (`calibration_ratio_x/y`).

The decompiled tree does not rebuild cleanly, so the kill switch was patched
out of the shipped binary with dnSpy instead — see
`ScopeMeterPatchedWithoutKillSwitch.zip` in the repository root.
