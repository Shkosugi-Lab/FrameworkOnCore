# Records the goldens of the System.Data.Linq parity cases from .NET Framework 4.8's own System.Data.Linq: builds
# DataLinqParity for net48 and runs every case, the database ones on the given SQL Server (FocDataLinqParity is
# created afresh there). Writes golden\cases.golden.json and golden\api.golden.txt, which
# tests/FrameworkOnCore.Tests (DataLinqParityTests) compares the port with. Windows only (net48).
#
#   .\tests\DataLinqParity\record.ps1 [-Connection "Data Source=.\SQLEXPRESS;Integrated Security=True"]
param([string]$Connection = 'Data Source=.\SQLEXPRESS;Initial Catalog=master;Integrated Security=True')
$ErrorActionPreference = 'Stop'
dotnet build $PSScriptRoot\DataLinqParity.csproj -f net48 -v q -nologo
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
$out = Join-Path ([IO.Path]::GetTempPath()) 'foc-dlinq-parity-net48'
& "$PSScriptRoot\bin\Debug\net48\DataLinqParity.exe" --connection $Connection --out $out
if ($LASTEXITCODE -ne 0) { throw 'the cases failed to run' }
New-Item -ItemType Directory "$PSScriptRoot\golden" -Force | Out-Null
Copy-Item "$out\cases.json" "$PSScriptRoot\golden\cases.golden.json" -Force
Copy-Item "$out\api.txt" "$PSScriptRoot\golden\api.golden.txt" -Force
"goldens -> $PSScriptRoot\golden"
