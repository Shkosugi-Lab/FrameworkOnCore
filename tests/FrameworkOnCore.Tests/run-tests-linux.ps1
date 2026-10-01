# Runs FrameworkOnCore.Tests on Linux (the .NET SDK's container): the compatibility assembly's helpers behave by
# platform (separators, the case of names, URIs, the Windows identity). The projects it builds are copied in, without
# their build output.
#   .\tests\FrameworkOnCore.Tests\run-tests-linux.ps1 [-SqlServer]
# -SqlServer: SQL Server in a container too (w2l-sql, as run-linux.ps1 keeps it), for the LINQ to SQL
# port's database tests (FOC_TEST_SQLSERVER); without it they are skipped.
# -Filter: those tests only (dotnet test --filter), each failure's message written out.
param([string]$Image = 'mcr.microsoft.com/dotnet/sdk:10.0', [switch]$SqlServer, [string]$Filter)
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
$script = @'
set -e
# System.Drawing's Linux implementation (the port) draws with libgdiplus.
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 >/dev/null 2>&1
for d in src/FrameworkOnCore.Analyzers src/FrameworkOnCore.Analysis src/FrameworkOnCore.Converter experiments/wf4c/shims/FrameworkOnCore.Compat tests/FrameworkOnCore.Tests tests/Parity.Core tests/DataLinqParity tests/DrawingParity tests/DataVisualizationParity \
         WebFormsForCore/src/WebFormsForCore.Data.Linq WebFormsForCore/src/WebFormsForCore.Drawing.Common WebFormsForCore/src/SigningKey WebFormsForCore/lib/WebFormsForCore.Build; do
  mkdir -p /w/$d
  (cd /repo/$d && find . -type f -not -path './bin/*' -not -path './obj/*' -exec cp --parents {} /w/$d/ \;)
done
# FrameworkOnCore's packages: the Chart control's parity program references the port as converted applications do.
mkdir -p /w/experiments/wf4c/_feed && cp /repo/experiments/wf4c/_feed/*.nupkg /w/experiments/wf4c/_feed/
# What the LINQ to SQL port's csproj links from its parent (FrameworkOnCore's version attributes).
cp /repo/WebFormsForCore/src/VersionInfo.cs /w/WebFormsForCore/src/
cd /w
dotnet test tests/FrameworkOnCore.Tests/FrameworkOnCore.Tests.csproj -nologo -v q __FILTER__
'@ -replace "`r", ''
$script = $script.Replace('__FILTER__', $(if ($Filter) { "--filter '$Filter' --logger 'console;verbosity=normal'" } else { '' }))
docker run --rm @arguments -v "${repo}:/repo:ro" $Image bash -c $script
