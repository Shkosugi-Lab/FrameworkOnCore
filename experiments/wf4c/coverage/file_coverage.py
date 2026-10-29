# The branches of some source files the unit tests of the port's own code do not take (tests/RuntimeUnitTests, or another of
# measure.py's programs with --program: SqlClient for tests/SqlClientTests): runs them under dotnet-coverage (measure.py
# <program>) unless --no-run, then lists each line with a branch not taken.
#
#   python experiments/wf4c/coverage/file_coverage.py [--program SqlClient] [--no-run] IisUrlRewrite.cs [IisHttpErrors.cs ...]
import os, subprocess, sys
import xml.etree.ElementTree as ET

here = os.path.dirname(os.path.abspath(__file__))
argv = sys.argv[1:]
program = 'Runtime'
if '--program' in argv:
    i = argv.index('--program'); program = argv[i + 1]; del argv[i:i + 2]
args = [a for a in argv if a != '--no-run']
if '--no-run' not in argv:
    subprocess.run([sys.executable, os.path.join(here, 'measure.py'), program], check=True)
root = ET.parse(os.path.join(here, 'results', program + '.cobertura.xml')).getroot()
lines = {}
for cls in root.iter('class'):
    path = cls.get('filename') or ''
    if not any(os.path.basename(path).lower() == a.lower() for a in args):
        continue
    for method in cls.iter('method'):
        for line in method.iter('line'):
            key = (path, int(line.get('number')))
            hits = int(line.get('hits', '0'))
            cond = line.get('condition-coverage')
            old = lines.get(key)
            lines[key] = (max(hits, old[0]) if old else hits, cond or (old[1] if old else None), method.get('name'))
for path in sorted({p for p, _ in lines}):
    text = open(path, encoding='utf-8-sig', errors='replace').read().splitlines()
    mine = sorted((n, v) for (p, n), v in lines.items() if p == path)
    total = len(mine); hit = sum(1 for _, v in mine if v[0] > 0)
    b = c = 0
    for _, v in mine:
        if v[1]:
            x, y = v[1].split('(')[1].rstrip(')').split('/'); c += int(x); b += int(y)
    print('== %s: lines %d/%d, branches %d/%d' % (os.path.basename(path), hit, total, c, b))
    for n, (hits, cond, method) in mine:
        if hits == 0 or (cond and cond.split('(')[1].split('/')[0] != cond.split('/')[1].rstrip(')')):
            print('%5d %-8s %s' % (n, 'unrun' if hits == 0 else cond.split(' ')[1], text[n - 1].strip()[:150]))
