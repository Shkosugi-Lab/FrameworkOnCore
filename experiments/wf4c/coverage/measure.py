# Line and branch coverage of FrameworkOnCore.Runtime's code by the unit tests and the parity programs (the differential tests against
# .NET Framework 4.8): each program run under dotnet-coverage with the packages' PDBs beside their DLLs, merged.
#
#   python experiments/wf4c/coverage/measure.py [Tests Runtime SqlClient Mobile DataVisualization Drawing WebControls[.TextBox]]
#
# Output: experiments/wf4c/coverage/results/<program>.cobertura.xml and merged.cobertura.xml.
import filecmp, glob, os, shutil, subprocess, sys

repo = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', '..', '..'))
results = os.path.join(repo, 'experiments', 'wf4c', 'coverage', 'results')
settings = os.path.join(repo, 'experiments', 'wf4c', 'coverage', 'collect.config')
packages = os.path.join(os.path.expanduser('~'), '.nuget', 'packages')
tests_bin = os.path.join(repo, 'tests', 'FrameworkOnCore.Tests', 'bin', 'Debug', 'net10.0')
version = '1.6.5-w2l.23'


def programs():
    tmp = os.path.join(results, 'out')
    # The unit tests (in their process: Drawing, DataLinq, the Compat shims...; the parity programs below again, alone).
    yield 'Tests', tests_bin, ['test', os.path.join(repo, 'tests', 'FrameworkOnCore.Tests', 'FrameworkOnCore.Tests.csproj'), '--no-build', '-nologo']
    # The unit tests of the port's own code (tests/RuntimeUnitTests: the runtime built from its source, with its PDBs).
    runtime = os.path.join(repo, 'tests', 'RuntimeUnitTests')
    yield 'Runtime', os.path.join(runtime, 'bin', 'Debug', 'net10.0'), ['test', os.path.join(runtime, 'RuntimeUnitTests.csproj'), '--no-build', '-nologo']
    # The functional tests of FrameworkOnCore's System.Data.SqlClient (tests/SqlClientTests: the port built from its source).
    sqlclient = os.path.join(repo, 'tests', 'SqlClientTests')
    yield 'SqlClient', os.path.join(sqlclient, 'bin', 'Debug', 'net10.0'), ['test', os.path.join(sqlclient, 'SqlClientTests.csproj'), '--no-build', '-nologo']
    yield 'Mobile', os.path.join(tests_bin, 'MobileParityData', 'program'), ['MobileParity.dll', '--out', os.path.join(tmp, 'mobile')]
    yield 'DataVisualization', os.path.join(tests_bin, 'DataVisualizationParityData', 'program'), ['DataVisualizationParity.dll', '--out', os.path.join(tmp, 'dv')]
    yield 'Drawing', os.path.join(tests_bin, 'DrawingParityData', 'program'), ['DrawingParity.dll', '--out', os.path.join(tmp, 'drawing')]
    wc = os.path.join(repo, 'tests', 'WebControlsParity', 'bin', 'Debug', 'net10.0')
    for control in sorted(os.listdir(os.path.join(repo, 'tests', 'WebControlsParity', 'golden'))):
        yield 'WebControls.' + control, wc, ['WebControlsParity.dll', 'api', '--control', control, '--out', os.path.join(tmp, 'wc', control)]


def add_pdbs(folder):
    """The runtime packages' PDBs beside the DLLs the program loads (NuGet copies the DLL only)."""
    for package in glob.glob(os.path.join(packages, 'frameworkoncore.*', version)):
        for pdb in glob.glob(os.path.join(package, 'lib', 'net10.0', '*.pdb')):
            dll = os.path.join(folder, os.path.basename(pdb)[:-4] + '.dll')
            # Only beside the package's own DLL (a project reference's has its own PDB).
            if os.path.exists(dll) and filecmp.cmp(dll, pdb[:-4] + '.dll', shallow=False):
                shutil.copy2(pdb, folder)


def main(selected):
    os.makedirs(results, exist_ok=True)
    done = []
    for name, folder, args in programs():
        if selected and not any(name == s or name.startswith(s + '.') for s in selected):
            continue
        if not os.path.isdir(folder):
            print(name, ': not built', folder); continue
        add_pdbs(folder)
        output = os.path.join(results, name + '.cobertura.xml')
        command = 'dotnet ' + ' '.join('"%s"' % a for a in args)
        r = subprocess.run(['dotnet-coverage', 'collect', command, '-s', settings, '-f', 'cobertura', '-o', output],
                           cwd=folder, capture_output=True, text=True, encoding='utf-8', errors='replace')
        ok = os.path.exists(output) and os.path.getsize(output) > 1000
        print(name, 'exit', r.returncode, 'coverage' if ok else 'NO COVERAGE')
        if not ok:
            print(r.stdout[-3000:], r.stderr[-3000:])
        else:
            done.append(output)
    if done and not selected:
        merged = os.path.join(results, 'merged.cobertura.xml')
        everything = sorted(glob.glob(os.path.join(results, '*.cobertura.xml')))
        everything = [f for f in everything if not f.endswith('merged.cobertura.xml')]
        subprocess.run(['dotnet-coverage', 'merge', '-f', 'cobertura', '-o', merged] + everything, check=True,
                       capture_output=True)
        print('merged', len(everything))
        # The components' files (124 of 40-80 MB) only for the merge.
        for f in everything:
            if os.path.basename(f).startswith('WebControls.'):
                os.remove(f)
        shutil.rmtree(os.path.join(results, 'out'), ignore_errors=True)


if __name__ == '__main__':
    main(sys.argv[1:])
