# Records the golden data of WingtipToys' operations suite on the ORIGINAL application (.NET Framework 4.8 on IIS):
# corpora\parity\wt.operations.golden.json, from the scenario corpora\regression\wt.operations.scenario.json (every
# operation of corpora\regression\wt.operations.json: corpora\regression\wt.operations.md is the readable list).
#
# Re-record (after changing the scenario; review the golden's diff before keeping it):
#   .\corpora\record-wt-operations.ps1                 # build the original, record, clean up
#   .\corpora\record-wt-operations.ps1 -Discover       # also crawl the original as each persona (anonymous, a new user,
#                                                      # the administrator) and check the inventory covers what it finds:
#                                                      # corpora\regression\wt.discovered.iis.json
#   .\corpora\record-wt-operations.ps1 -Keep           # leave the site running afterwards (to look at it; remove it with -Remove)
#   .\corpora\record-wt-operations.ps1 -Remove         # remove what -Keep left
#
# What it does:
#   1. builds the original (build-original.ps1 -Only wt: MSBuild of .NET Framework, Release);
#   2. a throwaway IIS site "foc-wtops" with its own application pool on a free port of 127.0.0.1 (8100-8199), anonymous
#      requests running as the pool's identity, which gets a SQL Server Express login, read access to the application
#      and write access to Catalog\Images (the administrator's image upload saves there);
#   3. the database reset to a known state: the pool stopped, the database WingtipToys dropped, the pool started and the
#      application's first request waited for - its Application_Start creates the database (EF 6 Code First: the
#      identity tables, the canEdit role and its user canEditUser@wingtiptoys.com / Pa$$word1; the catalogue's seed:
#      5 categories, 16 products) - exactly as the converted application's runs start (verify-windows.ps1 / run-linux.ps1
#      -Suite operations drop it the same way);
#   4. ParityTest record (with --sql: the scenario's state checks and preconditions read and set the database directly);
#   5. cleans up: the site, the pool, the SQL login, the uploaded images, the database, the ACL; SQL Server Express is
#      stopped again if this script started it.
# The application's connection strings point to .\SQLEXPRESS (record-webforms-golden.ps1 explains why: the original's
# Web.config names an anonymised Azure SQL server).
param([switch]$Keep, [switch]$Remove, [switch]$Discover, [switch]$SkipBuild,
    # Another scenario / output (to try a scenario out; the golden data stays as it is).
    [string]$Scenario, [string]$Out)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$physical = Join-Path $PSScriptRoot 'work\wingtiptoys-master\WingtipToys\WingtipToys'
$scenarioFile = if ($Scenario) { (Resolve-Path $Scenario).Path } else { Join-Path $PSScriptRoot 'regression\wt.operations.scenario.json' }
$inventory = Join-Path $PSScriptRoot 'regression\wt.operations.json'
$golden = if ($Out) { $Out } else { Join-Path $PSScriptRoot 'parity\wt.operations.golden.json' }
$name = 'foc-wtops'
$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
$sqlService = 'MSSQL$SQLEXPRESS'
$sqlConnection = 'Server=.\SQLEXPRESS;Database=WingtipToys;Integrated Security=true;TrustServerCertificate=true'
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
$images = Join-Path $physical 'Catalog\Images'

function Invoke-Sql([string]$sql, [string]$database = 'master') {
    sqlcmd -S .\SQLEXPRESS -E -C -b -d $database -Q $sql | Out-Null
    return $LASTEXITCODE -eq 0
}

function Remove-Everything {
    & $appcmd delete site $name 2>&1 | Out-Null
    & $appcmd delete apppool $name 2>&1 | Out-Null
    if ((Get-Service $sqlService).Status -eq 'Running') {
        Invoke-Sql "IF DB_ID(N'WingtipToys') IS NOT NULL BEGIN ALTER DATABASE [WingtipToys] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [WingtipToys]; END" | Out-Null
        Invoke-Sql "IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$name') DROP LOGIN [IIS APPPOOL\$name]" | Out-Null
    }
    Get-ChildItem $images -Recurse -Filter 'parity-*' -File -ErrorAction SilentlyContinue | Remove-Item -Force
    & icacls $images /remove "IIS AppPool\$name" /T /Q 2>&1 | Out-Null
    & icacls $physical /remove "IIS AppPool\$name" /Q 2>&1 | Out-Null
}

$startedSql = $false
if ((Get-Service $sqlService).Status -ne 'Running') { Start-Service $sqlService; $startedSql = $true }

if ($Remove) {
    Remove-Everything
    Stop-Service $sqlService
    Write-Host 'removed'
    exit 0
}

& { dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null }
if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
if (-not $SkipBuild) {
    & (Join-Path $PSScriptRoot 'build-original.ps1') -Only wt | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'the original did not build' }
}

# The connection strings as record-webforms-golden.ps1 sets them (already so after its first run).
$webConfig = Join-Path $physical 'Web.config'
$text = [IO.File]::ReadAllText($webConfig)
$replaced = [regex]::Replace($text, 'connectionString="Server=tcp:ms\.database\.windows\.net[^"]*"',
    'connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=WingtipToys;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30"')
if ($replaced -ne $text) { [IO.File]::WriteAllText($webConfig, $replaced, (New-Object Text.UTF8Encoding $true)) }

$port = 8100..8199 | Where-Object { -not (Get-NetTCPConnection -LocalPort $_ -State Listen -ErrorAction SilentlyContinue) } | Select-Object -First 1
$url = "http://127.0.0.1:$port"
Remove-Everything
& $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
& $appcmd add site /name:$name /physicalPath:$physical "/bindings:http/127.0.0.1:${port}:" | Out-Null
& $appcmd set app "$name/" /applicationPool:$name | Out-Null
& $appcmd set config "$name/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
# Read: the application (inherited by its files); modify: the upload's folder.
& icacls $physical /grant "IIS AppPool\${name}:(OI)(CI)(RX)" /Q 2>&1 | Out-Null
& icacls $images /grant "IIS AppPool\${name}:(OI)(CI)(M)" /T /Q 2>&1 | Out-Null
if (-not (Invoke-Sql "IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$name') CREATE LOGIN [IIS APPPOOL\$name] FROM WINDOWS; ALTER SERVER ROLE [sysadmin] ADD MEMBER [IIS APPPOOL\$name];")) {
    throw 'could not create the SQL login'
}

$exit = 0
$watch = [Diagnostics.Stopwatch]::StartNew()
try {
    # The known state: no database; the application's start creates it.
    & $appcmd stop apppool $name 2>&1 | Out-Null
    Invoke-Sql "IF DB_ID(N'WingtipToys') IS NOT NULL BEGIN ALTER DATABASE [WingtipToys] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [WingtipToys]; END" | Out-Null
    & $appcmd start apppool $name 2>&1 | Out-Null
    curl.exe -s -o NUL -m 300 --retry 10 --retry-connrefused --retry-delay 3 "$url/" | Out-Null
    Write-Host "original: $url"

    & dotnet $verifier record --url $url --scenario $scenarioFile --out $golden --mask $env:COMPUTERNAME --sql $sqlConnection
    $exit = $LASTEXITCODE
    if ($Discover) {
        & dotnet $verifier discover --url $url --out (Join-Path $PSScriptRoot 'regression\wt.discovered.iis.json') --inventory $inventory `
            --persona anonymous --persona register --persona 'login:canEditUser@wingtiptoys.com:Pa$$word1'
        if ($LASTEXITCODE -ne 0) { $exit = 1 }
    }
}
finally {
    Write-Host ("time: {0:mm\:ss}" -f $watch.Elapsed)
    if (-not $Keep) {
        Remove-Everything
        if ($startedSql) { Stop-Service $sqlService }
    }
    else { Write-Host "kept: $url (.\corpora\record-wt-operations.ps1 -Remove)" }
}
exit $exit
