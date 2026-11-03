# Runs a converted site as a supervisor does (systemd Restart=, Docker --restart, IIS): started again
# when it ends with exit code 75, WebFormsForCore's application restart (web.config or bin changed,
# HttpRuntime.UnloadAppDomain - an AppDomain recycle on .NET Framework).
#   .\experiments\wf4c\supervise.ps1 -Dll <site>\bin\<web>.dll [-Port 5096] [-Log <file>]
param([Parameter(Mandatory = $true)][string]$Dll, [int]$Port = 5096, [string]$Log,
    # The host of the URL it listens on (localhost: both loopbacks; 127.0.0.1: IPv4's only, as an IIS site bound to it).
    [string]$Address = 'localhost')

# A path relative to the current folder (bin\<web>.dll, run in the site's folder) as a full one: the site's folder below
# is the DLL's folder's parent (an empty one for "bin\<web>.dll" otherwise).
$Dll = (Resolve-Path -LiteralPath $Dll -ErrorAction Stop).Path
$directory = Split-Path (Split-Path $Dll -Parent) -Parent
if (-not $Log) { $Log = Join-Path $directory 'probe.log' }
do {
    # Through cmd: the output as the process writes it (UTF-8), stderr too, in one file.
    cmd /c "dotnet `"$Dll`" --urls http://${Address}:$Port >> `"$Log`" 2>&1"
    $code = $LASTEXITCODE
    [IO.File]::AppendAllText($Log, "=== supervise: exit $code$(if ($code -eq 75) { ', restarting' })`r`n")
} while ($code -eq 75)
