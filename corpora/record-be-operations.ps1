# Records the golden data of BlogEngine.NET's operations suite on the ORIGINAL application (.NET Framework 4.8 on IIS):
# corpora\parity\be.operations.golden.json, from the scenario corpora\regression\be.operations.scenario.json (every
# operation of corpora\regression\be.operations.json: corpora\regression\be.operations.md is the readable list).
#
# Re-record (after changing the scenario; review the golden's diff before keeping it):
#   .\corpora\record-be-operations.ps1                 # build the original, record, clean up (about 10 minutes)
#   .\corpora\record-be-operations.ps1 -Discover       # also crawl the original as each persona (anonymous, the
#                                                      # administrator, the editor the scenario made) and check the
#                                                      # inventory covers what it finds: corpora\regression\be.discovered.iis.json
#   .\corpora\record-be-operations.ps1 -Keep           # leave the site running afterwards (to look at it; remove it with -Remove)
#   .\corpora\record-be-operations.ps1 -Remove         # remove what -Keep left
#
# What it does:
#   1. builds the original (build-original.ps1 -Only be: MSBuild of .NET Framework);
#   2. a throwaway IIS site "foc-beops" with its own application pool on a free port of 127.0.0.1 (8100-8199), anonymous
#      requests running as the pool's identity, which gets write access to the application (as the converted runs' user
#      has: BlogEngine keeps everything in App_Data, writes Custom, and touches web.config to restart itself);
#   3. App_Data put back to the release's (corpora\be-app-data.ps1) with the pool stopped, then the pool started and the
#      first request waited for - exactly as the converted application's runs start (verify-windows.ps1 / run-linux.ps1
#      -Suite operations reset it the same way);
#   4. ParityTest record (with --files: the scenario's files steps read App_Data, the effect of each operation);
#   5. cleans up: the site, the pool, the ACLs; App_Data put back to the release's again.
param([switch]$Keep, [switch]$Remove, [switch]$Discover, [switch]$SkipBuild,
    # Another scenario / output (to try a scenario out; the golden data stays as it is).
    [string]$Scenario, [string]$Out)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$physical = Join-Path $PSScriptRoot 'work\BlogEngine.NET-3.3.8.0\BlogEngine\BlogEngine.NET'
$appData = Join-Path $physical 'App_Data'
$scenarioFile = if ($Scenario) { (Resolve-Path $Scenario).Path } else { Join-Path $PSScriptRoot 'regression\be.operations.scenario.json' }
$inventory = Join-Path $PSScriptRoot 'regression\be.operations.json'
$golden = if ($Out) { $Out } else { Join-Path $PSScriptRoot 'parity\be.operations.golden.json' }
$name = 'foc-beops'
$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
$resetAppData = Join-Path $PSScriptRoot 'be-app-data.ps1'

function Remove-Everything {
    & $appcmd delete site $name 2>&1 | Out-Null
    & $appcmd delete apppool $name 2>&1 | Out-Null
    & icacls $appData /remove "IIS AppPool\$name" /T /Q 2>&1 | Out-Null
    & icacls $physical /remove "IIS AppPool\$name" /Q 2>&1 | Out-Null
    & $resetAppData -Target $appData
}

if ($Remove) {
    Remove-Everything
    Write-Host 'removed'
    exit 0
}

& { dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null }
if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'build-original.ps1') -Only be | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'the original did not build' }
}

$port = 8100..8199 | Where-Object { -not (Get-NetTCPConnection -LocalPort $_ -State Listen -ErrorAction SilentlyContinue) } | Select-Object -First 1
$url = "http://127.0.0.1:$port"
Remove-Everything
& $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
& $appcmd add site /name:$name /physicalPath:$physical "/bindings:http/127.0.0.1:${port}:" | Out-Null
& $appcmd set app "$name/" /applicationPool:$name | Out-Null
& $appcmd set config "$name/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
# Modify: the application's folder, as the converted application's runs have it (their process's user's): BlogEngine
# writes App_Data, Custom (themes, widgets' files), and web.config - an extension enabled or disabled restarts the
# application by touching it (Utils.RecycleIIS); without the right that fails (logged, no restart) where the converted
# application restarts.
& icacls $physical /grant "IIS AppPool\${name}:(OI)(CI)(M)" /Q 2>&1 | Out-Null
& icacls $appData /grant "IIS AppPool\${name}:(OI)(CI)(M)" /T /Q 2>&1 | Out-Null

$exit = 0
$watch = [Diagnostics.Stopwatch]::StartNew()
try {
    # The known state: the release's App_Data, read by the application's start.
    & $appcmd stop apppool $name 2>&1 | Out-Null
    & $resetAppData -Target $appData
    & icacls $appData /grant "IIS AppPool\${name}:(OI)(CI)(M)" /T /Q 2>&1 | Out-Null
    & $appcmd start apppool $name 2>&1 | Out-Null
    curl.exe -s -o NUL -m 300 --retry 10 --retry-connrefused --retry-delay 3 "$url/" | Out-Null
    Write-Host "original: $url"

    # The application's folder (its tools page, its blogs' storage, the error pages name it), as written and JSON-escaped.
    & dotnet $verifier record --url $url --scenario $scenarioFile --out $golden --mask $env:COMPUTERNAME --files $appData `
        --mask ([regex]::Escape($physical.Replace('\', '\\'))) --mask ([regex]::Escape($physical))
    $exit = $LASTEXITCODE
    if ($Discover) {
        & dotnet $verifier discover --url $url --out (Join-Path $PSScriptRoot 'regression\be.discovered.iis.json') --inventory $inventory `
            --login-url /Account/login.aspx --persona anonymous --persona 'login:admin:Parity-5' --persona 'login:parity-editor:Parity-1'
        if ($LASTEXITCODE -ne 0) { $exit = 1 }
    }
}
finally {
    Write-Host ("time: {0:mm\:ss}" -f $watch.Elapsed)
    if (-not $Keep) { Remove-Everything }
    else { Write-Host "kept: $url (.\corpora\record-be-operations.ps1 -Remove)" }
}
exit $exit
