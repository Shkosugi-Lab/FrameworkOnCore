# Records the goldens of the System.Web.Mobile parity cases from .NET Framework 4.8's own assembly: builds
# MobileParity for net48 and runs every case. Writes golden\cases.golden.json and golden\api.golden.txt, which
# tests/FrameworkOnCore.Tests (MobileParityTests) compares the port with. Windows only (net48).
#   .\tests\MobileParity\record.ps1
$ErrorActionPreference = 'Stop'
dotnet build $PSScriptRoot\MobileParity.csproj -f net48 -v q -nologo
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
$out = Join-Path ([IO.Path]::GetTempPath()) 'foc-mobile-parity-net48'
& "$PSScriptRoot\bin\Debug\net48\MobileParity.exe" --out $out
if ($LASTEXITCODE -ne 0) { throw 'the cases failed to run' }
New-Item -ItemType Directory "$PSScriptRoot\golden" -Force | Out-Null
Copy-Item "$out\cases.json" "$PSScriptRoot\golden\cases.golden.json" -Force
Copy-Item "$out\api.txt" "$PSScriptRoot\golden\api.golden.txt" -Force
"goldens -> $PSScriptRoot\golden"