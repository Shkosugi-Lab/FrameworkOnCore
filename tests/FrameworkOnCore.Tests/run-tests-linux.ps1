# Runs FrameworkOnCore.Tests on Linux (the .NET SDK's container): the compatibility assembly's helpers behave by
# platform (separators, the case of names, URIs, the Windows identity). The projects it builds are copied in, without
# their build output.
#   .\tests\FrameworkOnCore.Tests\run-tests-linux.ps1 [-SqlServer]
# -SqlServer: SQL Server in a container too (w2l-sql, as run-linux.ps1 keeps it), for the LINQ to SQL
# port's database tests (FOC_TEST_SQLSERVER); without it they are skipped.
# -Filter: those tests only (dotnet test --filter), each failure's message written out.
# -DrawingBackend skia: System.Drawing over SkiaSharp instead of libgdiplus (FOC_DRAWING_BACKEND); -GdipTrace <file>: the
# GDI+ functions the tests called, with how many times, written to the file (FOC_GDIP_TRACE). With either, the Chart
# control's parity program (built against the packages) runs on the port as built here. -ParityStrict: the parity
# suites without Linux's known differences (FOC_PARITY_STRICT), to see how far its drawing is from .NET Framework's.
param([string]$Image = 'mcr.microsoft.com/dotnet/sdk:10.0', [switch]$SqlServer, [string]$Filter,
    [ValidateSet('', 'libgdiplus', 'skia')][string]$DrawingBackend, [string]$GdipTrace, [switch]$ParityStrict)
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$arguments = @()
if ($SqlServer) {
    $sqlPassword = 'W2l_local_Passw0rd'   # a throwaway local container
    docker network create w2l 2>$null | Out-Null
    if ((docker inspect -f '{{.State.Running}}' w2l-sql 2>$null) -ne 'true') {
        docker rm -f w2l-sql 2>$null | Out-Null
        docker run -d --name w2l-sql --network w2l -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$sqlPassword" mcr.microsoft.com/mssql/server:2022-latest | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'could not start SQL Server' }
    }
    foreach ($attempt in 1..60) {
        docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -Q 'SELECT 1' 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { break }
        Start-Sleep -Seconds 2
    }
    $arguments = @('--network', 'w2l', '-e', "FOC_TEST_SQLSERVER=Data Source=w2l-sql;Initial Catalog=master;User ID=sa;Password=$sqlPassword;TrustServerCertificate=True")
}
$drawing = [bool]($DrawingBackend -or $GdipTrace)
if ($DrawingBackend) { $arguments += @('-e', "FOC_DRAWING_BACKEND=$DrawingBackend") }
if ($ParityStrict) { $arguments += @('-e', 'FOC_PARITY_STRICT=1') }
if ($GdipTrace) {
    $traceDir = Split-Path ([IO.Path]::GetFullPath($GdipTrace))
    New-Item -ItemType Directory -Force $traceDir | Out-Null
    Remove-Item $GdipTrace -ErrorAction SilentlyContinue
    $arguments += @('-v', "${traceDir}:/trace", '-e', "FOC_GDIP_TRACE=/trace/$(Split-Path $GdipTrace -Leaf)")
}
$script = @'
set -e
# System.Drawing's Linux implementation (the port) draws with libgdiplus.
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 >/dev/null 2>&1
for d in src/FrameworkOnCore.Analyzers src/FrameworkOnCore.Analysis src/FrameworkOnCore.Converter src/FrameworkOnCore.Studio experiments/wf4c/shims/FrameworkOnCore.Compat tests/FrameworkOnCore.Tests tests/Parity.Core tests/DataLinqParity tests/DrawingParity tests/DataVisualizationParity tests/MobileParity tests/SqlClientTests \
         FrameworkOnCore.Runtime/src/WebFormsForCore.Data.Linq FrameworkOnCore.Runtime/src/WebFormsForCore.Data.SqlClient FrameworkOnCore.Runtime/src/WebFormsForCore.Drawing.Common FrameworkOnCore.Runtime/src/SigningKey FrameworkOnCore.Runtime/lib/WebFormsForCore.Build; do
  mkdir -p /w/$d
  (cd /repo/$d && find . -type f -not -path './bin/*' -not -path './obj/*' -exec cp --parents {} /w/$d/ \;)
done
# FrameworkOnCore's packages: the Chart control's and the mobile controls' parity programs reference the port as converted applications do.
mkdir -p /w/experiments/wf4c/_feed && cp /repo/experiments/wf4c/_feed/*.nupkg /w/experiments/wf4c/_feed/
# Their version: rules/packages.json's, read by FrameworkOnCoreVersion.props.
cp /repo/experiments/wf4c/FrameworkOnCoreVersion.props /w/experiments/wf4c/
# What the LINQ to SQL port's csproj links from its parent (FrameworkOnCore's version attributes).
cp /repo/FrameworkOnCore.Runtime/src/VersionInfo.cs /w/FrameworkOnCore.Runtime/src/
# The ports' satellites: FrameworkOnCore's translations, which their csproj import (FrameworkOnCore.Translations.targets);
# those of the ports copied only (the translation tests check each one against its sources: the others are not here).
cp /repo/FrameworkOnCore.Runtime/src/FrameworkOnCore.Translations.targets /w/FrameworkOnCore.Runtime/src/
mkdir -p /w/translations && cp /repo/translations/sources.json /w/translations/
for l in $(ls /repo/translations); do
  for a in System.Drawing.Common System.Data.SqlClient System.Data.Linq; do
    if [ -d /repo/translations/$l/$a ]; then mkdir -p /w/translations/$l && cp -r /repo/translations/$l/$a /w/translations/$l/; fi
  done
done
cd /w
__DRAWING__
dotnet test tests/FrameworkOnCore.Tests/FrameworkOnCore.Tests.csproj -nologo -v q __NOBUILD__ __FILTER__
__SQLCLIENT__
'@ -replace "`r", ''
$script = $script.Replace('__DRAWING__', $(if ($drawing) { @'
dotnet build tests/FrameworkOnCore.Tests/FrameworkOnCore.Tests.csproj -nologo -v q
for f in $(find tests/FrameworkOnCore.Tests/bin -path '*DataVisualizationParityData/program/runtimes/unix/*' -name System.Drawing.Common.dll); do
  cp FrameworkOnCore.Runtime/src/WebFormsForCore.Drawing.Common/bin/unix/*/net10.0/System.Drawing.Common.dll $f
done
# The port's SkiaSharp beside the program, listed in its deps.json (the packages it was built with have none; no
# double quotes here: Windows PowerShell passes them to docker unescaped).
for p in $(find tests/FrameworkOnCore.Tests/bin -type d -path '*DataVisualizationParityData/program'); do
  t=$(dirname $(find tests/FrameworkOnCore.Tests/bin -name SkiaSharp.dll -not -path '*DataVisualizationParityData*' | head -1))
  cp $t/SkiaSharp.dll $p/ && cp $t/runtimes/linux-x64/native/libSkiaSharp.so $p/
  sed -i -e '0,/.\.NETCoreApp,Version=v10\.0.: {/s//\x22.NETCoreApp,Version=v10.0\x22: { \x22FocSkiaSharp\/1.0.0\x22: { \x22runtime\x22: { \x22SkiaSharp.dll\x22: {} }, \x22native\x22: { \x22libSkiaSharp.so\x22: {} } },/' \
         -e '0,/.libraries.: {/s//\x22libraries\x22: { \x22FocSkiaSharp\/1.0.0\x22: { \x22type\x22: \x22project\x22, \x22serviceable\x22: false, \x22sha512\x22: \x22\x22 },/' $p/DataVisualizationParity.deps.json
done
'@ -replace "`r", '' } else { '' }))
$script = $script.Replace('__NOBUILD__', $(if ($drawing) { '--no-build' } else { '' }))
$script = $script.Replace('__FILTER__', $(if ($Filter) { "--filter '$Filter' --logger 'console;verbosity=normal'" } else { '' }))
# The System.Data.SqlClient port's tests (Microsoft's functional tests and |DataDirectory|'s, with their TDS server) on
# the managed SNI, Linux's; with -Filter, FrameworkOnCore.Tests' only.
$script = $script.Replace('__SQLCLIENT__', $(if ($Filter) { '' } else { 'dotnet test tests/SqlClientTests/SqlClientTests.csproj -nologo -v q' }))
docker run --rm @arguments -v "${repo}:/repo:ro" $Image bash -c $script
