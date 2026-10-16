"""The GC's numbers while the load ran (dotnet-counters collect --format csv, perf-check.ps1 -Diagnose): each counter's mean
over the samples (rates per second, sizes and percentages as they were), the counters about the GC and allocations.
    python perf-counters.py <counters.csv>"""
import csv
import sys
from collections import defaultdict

values = defaultdict(list)
with open(sys.argv[1], encoding='utf-8-sig') as f:
    for row in csv.DictReader(f):
        try:
            values[(row['Counter Name'], row.get('Counter Type', ''))].append(float(row['Mean/Increment']))
        except (KeyError, ValueError):
            continue
words = ('gc', 'GC', 'alloc', 'Alloc', 'heap', 'Heap', 'Gen', 'gen', 'pause', 'Pause', 'CPU', 'cpu', 'thread', 'Thread', 'finaliz', 'Finaliz', 'working', 'Working')
for (name, kind), samples in sorted(values.items()):
    if not any(w in name for w in words):
        continue
    print('%-70s %-8s %14.1f' % (name[:70], kind[:8], sum(samples) / len(samples)))
