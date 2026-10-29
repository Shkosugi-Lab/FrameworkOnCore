# The branches of the port's own code (static analysis of FrameworkOnCore.Runtime's sources) and which of them the tests
# reach (coverage from measure.py).
#
# The port's code: the reference source's lines under a preprocessor condition of the port (#if WebFormsForCore,
# NETCOREAPP, !NETFRAMEWORK, NET8_0_OR_GREATER, TARGET_UNIX... and the #else of #if NETFRAMEWORK / !WebFormsForCore), and
# the files the port added (the WebFormsForCore and Core folders). The coverage lists the lines compiled into the
# net10.0 build only, so a line there under such a condition is in its active part: the condition need not be evaluated.
#
#   python experiments/wf4c/coverage/port_branches.py [cobertura]  ->  results/port-branches.md, results/port-branches.json
import json, os, re, sys
import xml.etree.ElementTree as ET
from collections import defaultdict

here = os.path.dirname(os.path.abspath(__file__))
repo = os.path.abspath(os.path.join(here, '..', '..', '..'))
src = os.path.join(repo, 'FrameworkOnCore.Runtime', 'src')
cobertura = sys.argv[1] if len(sys.argv) > 1 else os.path.join(here, 'results', 'merged.cobertura.xml')

PORT_SYMBOL = re.compile(r'\b(WebFormsForCore|NETCOREAPP|NETFRAMEWORK|NET\d+_\d+(_OR_GREATER)?|NETSTANDARD[\d_]*|NET|TARGET_UNIX|TARGET_WINDOWS|DebugWF4C)\b')
PORT_FOLDER = re.compile(r'[\\/](WebFormsForCore|Core)[\\/]')
DIRECTIVE = re.compile(r'^\s*#\s*(if|elif|else|endif)\b(.*)$')


def port_lines(path):
    """The 1-based lines of a source file that are the port's (see above)."""
    if PORT_FOLDER.search(os.path.relpath(path, src)):
        return None  # every line
    lines = set()
    stack = []  # per open #if: is any part of its group conditioned on a port symbol
    try:
        text = open(path, encoding='utf-8-sig', errors='replace').read().splitlines()
    except OSError:
        return set()
    for number, line in enumerate(text, 1):
        m = DIRECTIVE.match(line)
        if m:
            kind, condition = m.group(1), m.group(2).split('//')[0]
            if kind == 'if':
                stack.append(bool(PORT_SYMBOL.search(condition)))
            elif kind == 'elif' and stack:
                stack[-1] = stack[-1] or bool(PORT_SYMBOL.search(condition))
            elif kind == 'endif' and stack:
                stack.pop()
            continue
        if any(stack):
            lines.add(number)
    return lines


def main():
    root = ET.parse(cobertura).getroot()
    cache = {}
    per_assembly = defaultdict(lambda: [0, 0, 0, 0])  # lines, covered, branches, covered branches
    uncovered = defaultdict(list)  # assembly -> (file, line, method, covered/total)
    for package in root.iter('package'):
        assembly = package.get('name')
        seen = set()
        for cls in package.iter('class'):
            path = cls.get('filename')
            if not path or not path.lower().startswith(src.lower()):
                continue
            if path not in cache:
                cache[path] = port_lines(path)
            port = cache[path]
            for method in cls.iter('method'):
                for line in method.iter('line'):
                    number = int(line.get('number'))
                    if (path, number) in seen or (port is not None and number not in port):
                        continue
                    seen.add((path, number))
                    totals = per_assembly[assembly]
                    totals[0] += 1
                    hit = int(line.get('hits', '0')) > 0
                    totals[1] += hit
                    cond = line.get('condition-coverage')
                    if cond:
                        a, b = map(int, cond.split('(')[1].rstrip(')').split('/'))
                        totals[2] += b
                        totals[3] += a
                        if a < b:
                            uncovered[assembly].append((os.path.relpath(path, src), number,
                                                        cls.get('name') + '.' + method.get('name'), a, b, hit))
    out = os.path.join(here, 'results')
    pct = lambda a, b: '%.1f' % (100.0 * a / b) if b else '-'
    report = ['# 移植で変えたコードの分岐網羅(port_branches.py)', '',
              '| アセンブリ | 行 | 通った行 | % | 分岐 | 通った分岐 | % |', '|---|---:|---:|---:|---:|---:|---:|']
    for assembly, (l, lc, b, bc) in sorted(per_assembly.items(), key=lambda kv: -kv[1][2]):
        report.append('| %s | %d | %d | %s | %d | %d | %s |' % (assembly, l, lc, pct(lc, l), b, bc, pct(bc, b)))
    total = [sum(v[i] for v in per_assembly.values()) for i in range(4)]
    report.append('| 計 | %d | %d | %s | %d | %d | %s |' % (total[0], total[1], pct(total[1], total[0]), total[2], total[3], pct(total[3], total[2])))
    for assembly, items in sorted(uncovered.items()):
        report += ['', '## ' + assembly, '', '| ファイル | 行 | メソッド | 分岐 | 行に到達 |', '|---|---:|---|---:|---|']
        for f, n, m, a, b, hit in sorted(items):
            report.append('| %s | %d | %s | %d/%d | %s |' % (f, n, m.replace('|', '\\|'), a, b, 'yes' if hit else 'no'))
    open(os.path.join(out, 'port-branches.md'), 'w', encoding='utf-8').write('\n'.join(report) + '\n')
    json.dump({a: [dict(file=f, line=n, method=m, covered=c, branches=b, reached=h) for f, n, m, c, b, h in items]
               for a, items in uncovered.items()}, open(os.path.join(out, 'port-branches.json'), 'w', encoding='utf-8'), indent=1)
    print('\n'.join(report[:4 + len(per_assembly) + 1]))


main()
