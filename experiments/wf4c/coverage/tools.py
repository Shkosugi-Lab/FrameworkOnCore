# Line and branch coverage of the tools written new (the converter, its analysis and analyzers, Studio) by the unit tests:
#   python experiments/wf4c/coverage/tools.py --collect            (runs the tests under dotnet-coverage: results/tools.cobertura.xml)
#   python experiments/wf4c/coverage/tools.py [file-name-part]     (the summary; a file: its lines no test runs)
# Each assembly's and each file's lines and branches, the least covered first.
# --collect runs the tests without the compiler's server and MSBuild's kept nodes (UseSharedCompilation=false,
# MSBUILDDISABLENODEREUSE=1): the builds the tests start (the converted applications') would start them under the
# collector, they outlive it, and the next dotnet build that finds the compiler's server waits on it without end.
import os, re, sys, collections
import xml.etree.ElementTree as ET

here = os.path.dirname(os.path.abspath(__file__))
if len(sys.argv) > 1 and sys.argv[1] == '--collect':
    import subprocess
    repo = os.path.abspath(os.path.join(here, '..', '..', '..'))
    environment = dict(os.environ, UseSharedCompilation='false', MSBUILDDISABLENODEREUSE='1', DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER='1')
    code = subprocess.call(['dotnet-coverage', 'collect', '-s', os.path.join(here, 'collect-tools.config'), '-f', 'cobertura',
                            '-o', os.path.join(here, 'results', 'tools.cobertura.xml'),
                            'dotnet test tests/FrameworkOnCore.Tests/FrameworkOnCore.Tests.csproj --no-build -nologo'], cwd=repo, env=environment)
    subprocess.call(['dotnet', 'build-server', 'shutdown'], cwd=repo, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    sys.exit(code)
tree = ET.parse(os.path.join(here, 'results', 'tools.cobertura.xml'))
files = collections.defaultdict(lambda: {'lines': {}, 'branches': {}})
for package in tree.getroot().iter('package'):
    for cls in package.iter('class'):
        name = cls.get('filename').replace(chr(92), '/')
        if '/obj/' in name or '/src/' not in name: continue
        key = (package.get('name'), name[name.index('/src/') + 5:])
        for line in cls.iter('line'):
            n = int(line.get('number'))
            files[key]['lines'][n] = max(files[key]['lines'].get(n, 0), int(line.get('hits')))
            m = re.match(r'\d+% \((\d+)/(\d+)\)', line.get('condition-coverage') or '')
            if m:
                old = files[key]['branches'].get(n, (0, 0))
                files[key]['branches'][n] = (max(old[0], int(m.group(1))), max(old[1], int(m.group(2))))

def numbers(entry):
    lines = entry['lines']; branches = entry['branches']
    return (sum(1 for h in lines.values() if h), len(lines), sum(b[0] for b in branches.values()), sum(b[1] for b in branches.values()))

if len(sys.argv) > 1:
    for (package, name), entry in files.items():
        if sys.argv[1].lower() in name.lower():
            missed = sorted(n for n, h in entry['lines'].items() if not h)
            partial = sorted(n for n, b in entry['branches'].items() if b[0] < b[1] and entry['lines'].get(n))
            print(name, numbers(entry)); print('  not run:', missed); print('  branches not all taken:', partial)
    sys.exit(0)

by_package = collections.defaultdict(lambda: [0, 0, 0, 0])
rows = []
for (package, name), entry in files.items():
    n = numbers(entry)
    for i in range(4): by_package[package][i] += n[i]
    rows.append((n[1] - n[0], name, n))
print('%-34s %8s %8s' % ('assembly', 'lines', 'branches'))
for package, n in sorted(by_package.items()):
    print('%-34s %5.1f%% (%d/%d) %5.1f%% (%d/%d)' % (package, 100.0 * n[0] / max(n[1], 1), n[0], n[1], 100.0 * n[2] / max(n[3], 1), n[2], n[3]))
print()
for missed, name, n in sorted(rows, reverse=True)[:45]:
    print('%-62s lines %5.1f%% (%4d not run)  branches %5.1f%% (%d/%d)' % (name, 100.0 * n[0] / max(n[1], 1), missed, 100.0 * n[2] / max(n[3], 1), n[2], n[3]))
