import json
import sys
from matplotlib_cs import MatplotlibCS

# Script builds a matplotlib figure based on information, passed to it through json file.
# Path to the JSON must be passed in first command line argument.

def main(filepath):
    with open(filepath, 'r') as f:
        s = f.read()
        task = json.loads(s)
        mpl = MatplotlibCS(task)
        mpl.build_figure()

print("here")

if __name__ == '__main__':
    main(sys.argv[1])
