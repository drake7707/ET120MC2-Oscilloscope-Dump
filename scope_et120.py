#!/usr/bin/env python3
"""Launcher for the et120 package.

    scope_et120.py ports                    # find the instrument
    scope_et120.py info --plot              # connect, report and plot one acquisition
    scope_et120.py capture -o signal        # -> signal.pwl for LTspice
    scope_et120.py stored                   # what is saved on the instrument
    scope_et120.py stored 4 -o event --all  # fetch slot 4, write .pwl/.csv/.raw
    scope_et120.py decode dump.bin -o sig   # decode a saved raw dump, no hardware
    scope_et120.py limits                   # what the hardware can and cannot do

Equivalent to 'python -m et120'. Requires pyserial and numpy; matplotlib only
for --plot.
"""

import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from et120.cli import main    # noqa: E402

if __name__ == "__main__":
    sys.exit(main())
