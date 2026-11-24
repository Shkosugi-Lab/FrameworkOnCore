# The converter (FrameworkOnCore.Converter) of this checkout's src/: the build GitHub Actions published, fetched once
# (tools/foc-tools.ps1), or built here when there is none. Its arguments as they are; FrameworkOnCore's runtime
# (experiments/wf4c) of this checkout unless --runtime is given.
#
#   .\converter.ps1 analyze <Web project> --out <folder>
#   .\converter.ps1 <Web project> --out <folder> [--choices foc-choices.json] [--build-original]
#   .\converter.ps1 -Build ...       # built here
# No param block: the arguments are the tool's, as they are ($args). With one, Windows PowerShell started as
# "powershell -File <script> <project> --out <folder>" took --out for a parameter of its own (-OutVariable, -OutBuffer:
# "the parameter name 'out' is ambiguous") and --b... for -Build. -Build alone (as written) is the script's.
$Build = @($args | Where-Object { $_ -is [string] -and $_ -ceq '-Build' }).Count -gt 0
$Arguments = @($args | Where-Object { -not ($_ -is [string] -and $_ -ceq '-Build') } | ForEach-Object { "$_" })

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'tools\foc-tools.ps1')
$tools = Get-FocTools -Build:$Build
# Only the conversion and analyze take --runtime: build-original, build-original-advice and snapshot would read it as
# the project (the last argument without an option is the project).
$takesRuntime = -not ($Arguments.Count -gt 0 -and $Arguments[0] -in 'build-original', 'build-original-advice', 'snapshot')
$runtime = if ($Arguments -contains '--runtime' -or -not $takesRuntime) { @() } else { @('--runtime', (Join-Path $PSScriptRoot 'experiments\wf4c')) }
$Arguments = @(ConvertTo-FocNativeArguments $Arguments)
dotnet (Join-Path $tools 'FrameworkOnCore.Converter.dll') @Arguments @runtime
exit $LASTEXITCODE
