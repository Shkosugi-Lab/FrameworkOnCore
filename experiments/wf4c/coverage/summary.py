# Coverage per assembly from a cobertura file (measure.py): lines and branches covered.
#   python experiments/wf4c/coverage/summary.py [file]
import os, sys
import xml.etree.ElementTree as ET

path = sys.argv[1] if len(sys.argv) > 1 else os.path.join(os.path.dirname(__file__), 'results', 'merged.cobertura.xml')
root = ET.parse(path).getroot()
print('%-45s %9s %9s %7s %9s %9s %7s' % ('assembly', 'lines', 'covered', '%', 'branches', 'covered', '%'))
for package in root.iter('package'):
    lines = covered = branches = bcovered = 0
    for line in package.iter('line'):
        lines += 1
        if int(line.get('hits', '0')) > 0:
            covered += 1
        cond = line.get('condition-coverage')
        if cond:
            a, b = cond.split('(')[1].rstrip(')').split('/')
            bcovered += int(a); branches += int(b)
    pct = lambda a, b: '%.1f' % (100.0 * a / b) if b else '-'
    print('%-45s %9d %9d %7s %9d %9d %7s' % (package.get('name'), lines, covered, pct(covered, lines), branches, bcovered, pct(bcovered, branches)))
