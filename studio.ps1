# FrameworkOnCore Studio (the analysis, the choices, the conversion in a local web UI): the build of this checkout's
# src/ published by GitHub Actions, fetched once (tools/foc-tools.ps1), or built here when there is none.
#
#   .\studio.ps1                     # then open http://127.0.0.1:5300/ (localhost only)
#   .\studio.ps1 -Build              # built here (a change to src/ not committed is built anyway)
#   .\studio.ps1 --port 5301         # Studio's options (--port, --data, --runtime)
param([switch]$Build, [Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments = @())

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'tools\foc-tools.ps1')
$tools = Get-FocTools -Build:$Build
$runtime = if ($Arguments -contains '--runtime') { @() } else { @('--runtime', (Join-Path $PSScriptRoot 'experiments\wf4c')) }
$port = if ($Arguments -contains '--port') { $Arguments[[array]::IndexOf($Arguments, '--port') + 1] } else { 5300 }
Write-Host "Studio: http://127.0.0.1:$port/ ($tools)"
dotnet (Join-Path $tools 'FrameworkOnCore.Studio.dll') @runtime @Arguments
exit $LASTEXITCODE
