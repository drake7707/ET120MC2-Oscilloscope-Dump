"""Allow 'python -m et120 ...' as well as './scope_et120.py ...'."""

import sys

from .cli import main

if __name__ == "__main__":
    sys.exit(main())
