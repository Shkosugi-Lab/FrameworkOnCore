# Puts BlogEngine.NET's App_Data back to the state of its release (v3.3.8.0): the known state every run of be's
# operations suite starts from (record-be-operations.ps1 on IIS, verify-windows.ps1 -Name be -Suite operations,
# run-linux.ps1 ... -Suite operations). BlogEngine keeps everything there (posts, pages, comments in the posts' files,
# categories, users, roles, rights, settings, widgets, profiles, the extensions' settings, uploaded files): a run's
# operations change it, and the next run would start from what the last one left.
#
# The pristine copy is the release's App_Data, taken from the release archive (corpora\fetch.ps1's URL), kept in
# corpora\work\_pristine\be-App_Data (not in Git: the corpora are fetched, not redistributed). The folder given is made
# the same as it (files the run made removed, files it changed restored), except the files the conversion adds
# (App_Data\culture-profile.json, App_Data\machine.config: kept).
#
#   .\corpora\be-app-data.ps1 -Target <web project>\App_Data
#   .\corpora\be-app-data.ps1 -Target ... -Check     # only say whether it is pristine (exit 1 if not)
param([Parameter(Mandatory = $true)][string]$Target, [switch]$Check)

$ErrorActionPreference = 'Stop'
$pristine = Join-Path $PSScriptRoot 'work\_pristine\be-App_Data'
if (-not (Test-Path (Join-Path $pristine 'settings.xml'))) {
    $archive = Join-Path $env:TEMP 'foc-be-3.3.8.0.zip'
    $extract = Join-Path $env:TEMP 'foc-be-3.3.8.0'
    Write-Host 'pristine App_Data: from the release archive (once)'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Invoke-WebRequest 'https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip' -OutFile $archive -UseBasicParsing
    if (Test-Path $extract) { Remove-Item $extract -Recurse -Force }
    Expand-Archive $archive $extract
    New-Item -ItemType Directory (Split-Path $pristine) -Force | Out-Null
    if (Test-Path $pristine) { Remove-Item $pristine -Recurse -Force }
    Copy-Item (Join-Path $extract 'BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET\App_Data') $pristine -Recurse
    Remove-Item $extract, $archive -Recurse -Force
}
$keep = @('culture-profile.json', 'machine.config')
if ($Check) {
    $output = & robocopy $pristine $Target /MIR /L /NJH /NJS /NDL /NP /FP /XF @keep 2>&1
    $changed = @($output | Where-Object { $_ -match '\S' })
    if ($changed.Count -eq 0) { Write-Host "pristine: $Target"; exit 0 }
    $changed | Select-Object -First 30 | ForEach-Object { Write-Host "  $_" }
    exit 1
}
New-Item -ItemType Directory $Target -Force | Out-Null
# /MIR: the same files and folders as the pristine copy; /IS /IT: copied even when the size and time agree (a file
# rewritten within the same second); /XF: the conversion's own files stay.
& robocopy $pristine $Target /MIR /IS /IT /R:3 /W:1 /NJH /NJS /NDL /NFL /NP /XF @keep | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE): $Target" }
exit 0
