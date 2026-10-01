# Records the goldens of the System.Web.DataVisualization parity cases from .NET Framework 4.8's own assembly: builds
# DataVisualizationParity for net48 and runs every case. Writes golden\cases.golden.json and golden\api.golden.txt, which
# tests/FrameworkOnCore.Tests (DataVisualizationParityTests) compares the port with. Windows only (net48).
#   .\tests\DataVisualizationParity\record.ps1
$ErrorActionPreference = 'Stop'
dotnet build $PSScriptRoot\DataVisualizationParity.csproj -f net48 -v q -nologo
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
$out = Join-Path ([IO.Path]::GetTempPath()) 'foc-dataviz-parity-net48'
& "$PSScriptRoot\bin\Debug\net48\DataVisualizationParity.exe" --out $out
if ($LASTEXITCODE -ne 0) { throw 'the cases failed to run' }
New-Item -ItemType Directory "$PSScriptRoot\golden" -Force | Out-Null
Copy-Item "$out\cases.json" "$PSScriptRoot\golden\cases.golden.json" -Force
Copy-Item "$out\api.txt" "$PSScriptRoot\golden\api.golden.txt" -Force
"goldens -> $PSScriptRoot\golden"