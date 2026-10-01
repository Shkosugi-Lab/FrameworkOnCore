# The converter (FrameworkOnCore.Converter) of this checkout's src/: the build GitHub Actions published, fetched once
# (tools/foc-tools.ps1), or built here when there is none. Its arguments as they are; FrameworkOnCore's runtime
# (experiments/wf4c) of this checkout unless --runtime is given.
#
#   .\converter.ps1 analyze <Web project> --out <folder>
#   .\converter.ps1 <Web project> --out <folder> [--choices foc-choices.json] [--build-original]
#   .\converter.ps1 -Build ...       # built here
param([switch]$Build, [Parameter(ValueFromRemainingArguments = $true)][string[]]$Arguments = @())

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'tools\foc-tools.ps1')
$tools = Get-FocTools -Build:$Build
$runtime = if ($Arguments -contains '--runtime') { @() } else { @('--runtime', (Join-Path $PSScriptRoot 'experiments\wf4c')) }
dotnet (Join-Path $tools 'FrameworkOnCore.Converter.dll') @Arguments @runtime
exit $LASTEXITCODE
