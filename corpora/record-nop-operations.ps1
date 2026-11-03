# Records the golden data of nopCommerce 1.90's operations suite on the ORIGINAL application (.NET Framework 4.8 on IIS):
# corpora\parity\nop.operations.golden.json, from the scenario corpora\regression\nop.operations.scenario.json (every
# operation of corpora\regression\nop.operations.json: corpora\regression\nop.operations.md is the readable list).
#
# Re-record (after changing the scenario; review the golden's diff before keeping it):
#   .\corpora\record-nop-operations.ps1                 # record, clean up (about 35 minutes)
#   .\corpora\record-nop-operations.ps1 -Discover       # also crawl the original as each persona (anonymous, the scenario's
#                                                       # customer, the administrator) and check the inventory covers what it
#                                                       # finds: corpora\regression\nop.discovered.iis.json
#   .\corpora\record-nop-operations.ps1 -Keep           # leave the site running afterwards (to look at it; remove it with -Remove)
#   .\corpora\record-nop-operations.ps1 -Remove         # remove what -Keep left
#
# What it does:
#   1. the original as its build deployed it: experiments\wf4c\nop.original\NopCommerceStore (the converter's build of the
#      original solution: .\experiments\wf4c\convert-corpora.ps1 -Only nop makes it), COPIED to corpora\work\_iis\nop - the
#      application writes into its folder (ConnectionStrings.config is the installer's to write; images\thumbs, the
#      pictures' resized copies; files\exportimport, the exports): the build stays as it was, each recording starts from it;
#   2. a throwaway IIS site "foc-nopops" with its own application pool on a free port of 127.0.0.1 (8100-8199), anonymous
#      requests running as the pool's identity, which gets a SQL Server Express login (sysadmin, as wt's) and write access
#      to the copy;
#   3. the database reset to the known state (corpora\nop-database.ps1: the installer's sample data, restored from its
#      backup) with the pool stopped, the connection string written as the installer writes it (ConnectionStrings.config),
#      then the pool started and the first request waited for - exactly as the converted application's runs start
#      (verify-windows.ps1 / run-linux.ps1 -Suite operations reset it the same way);
#   4. ParityTest record (with --sql: the scenario's state checks and preconditions read and set the database directly);
#   5. cleans up: the site, the pool, the SQL login, the copy; SQL Server Express is stopped again if this script started it.
param([switch]$Keep, [switch]$Remove, [switch]$Discover,
    # Another scenario / output (to try a scenario out; the golden data stays as it is).
    [string]$Scenario, [string]$Out)

$ErrorActionPreference = 'Continue'
$repo = Split-Path $PSScriptRoot -Parent
$build = Join-Path $repo 'experiments\wf4c\nop.original\NopCommerceStore'
$physical = Join-Path $PSScriptRoot 'work\_iis\nop'
$scenarioFile = if ($Scenario) { (Resolve-Path $Scenario).Path } else { Join-Path $PSScriptRoot 'regression\nop.operations.scenario.json' }
$inventory = Join-Path $PSScriptRoot 'regression\nop.operations.json'
$golden = if ($Out) { $Out } else { Join-Path $PSScriptRoot 'parity\nop.operations.golden.json' }
$name = 'foc-nopops'
$database = 'nopCommerceOps'
$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
$sqlService = 'MSSQL$SQLEXPRESS'
$sqlConnection = "Server=.\SQLEXPRESS;Database=$database;Integrated Security=true;TrustServerCertificate=true"
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'

function Invoke-Sql([string]$sql) {
    sqlcmd -S .\SQLEXPRESS -E -C -b -Q $sql | Out-Null
    return $LASTEXITCODE -eq 0
}

function Remove-Everything {
    & $appcmd delete site $name 2>&1 | Out-Null
    & $appcmd delete apppool $name 2>&1 | Out-Null
    if ((Get-Service $sqlService).Status -eq 'Running') {
        Invoke-Sql "IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$name') DROP LOGIN [IIS APPPOOL\$name]" | Out-Null
    }
    if (Test-Path $physical) { cmd /c rmdir /s /q "$physical" 2>&1 | Out-Null }
}

$startedSql = $false
if ((Get-Service $sqlService).Status -ne 'Running') { Start-Service $sqlService; $startedSql = $true }

if ($Remove) {
    Remove-Everything
    & (Join-Path $PSScriptRoot 'nop-database.ps1') -Drop
    if ($startedSql) { Stop-Service $sqlService }
    Write-Host 'removed'
    exit 0
}

if (-not (Test-Path (Join-Path $build 'bin\NopCommerceStore.dll'))) {
    throw "$build is not built: .\experiments\wf4c\convert-corpora.ps1 -Only nop builds the original (its .original folder)"
}
& { dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null }
if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }

$port = 8100..8199 | Where-Object { -not (Get-NetTCPConnection -LocalPort $_ -State Listen -ErrorAction SilentlyContinue) } | Select-Object -First 1
$url = "http://127.0.0.1:$port"
Remove-Everything
New-Item -ItemType Directory $physical -Force | Out-Null
& robocopy $build $physical /MIR /R:3 /W:1 /NJH /NJS /NDL /NFL /NP | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy failed ($LASTEXITCODE)" }
# The connection string as the installer writes it (InstallerHelper.SaveConnectionString: NopSqlConnection).
[IO.File]::WriteAllText((Join-Path $physical 'ConnectionStrings.config'), "<connectionStrings>`r`n  <add name=`"NopSqlConnection`" connectionString=`"Data Source=.\SQLEXPRESS;Initial Catalog=$database;Integrated Security=True;Persist Security Info=False;MultipleActiveResultSets=True;Connect Timeout=120`" />`r`n</connectionStrings>")
& $appcmd add apppool /name:$name /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
& $appcmd add site /name:$name /physicalPath:$physical "/bindings:http/127.0.0.1:${port}:" | Out-Null
& $appcmd set app "$name/" /applicationPool:$name | Out-Null
& $appcmd set config "$name/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
# Modify: the copy (the pictures' thumbnails, the exports, the feeds the application writes), as the converted runs' user has it.
& icacls $physical /grant "IIS AppPool\${name}:(OI)(CI)(M)" /Q 2>&1 | Out-Null
if (-not (Invoke-Sql "IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\$name') CREATE LOGIN [IIS APPPOOL\$name] FROM WINDOWS; ALTER SERVER ROLE [sysadmin] ADD MEMBER [IIS APPPOOL\$name];")) {
    throw 'could not create the SQL login'
}

$exit = 0
$watch = [Diagnostics.Stopwatch]::StartNew()
try {
    # The known state: the sample store's database, read by the application's start.
    & $appcmd stop apppool $name 2>&1 | Out-Null
    & (Join-Path $PSScriptRoot 'nop-database.ps1')
    if ($LASTEXITCODE -ne 0) { throw 'could not restore the database' }
    & $appcmd start apppool $name 2>&1 | Out-Null
    curl.exe -s -o NUL -m 300 --retry 10 --retry-connrefused --retry-delay 3 "$url/default.aspx" | Out-Null
    Write-Host "original: $url"

    # The application's folder (the error pages and the system information name it), as written and JSON-escaped.
    & dotnet $verifier record --url $url --scenario $scenarioFile --out $golden --sql $sqlConnection --mask $env:COMPUTERNAME `
        --mask ([regex]::Escape($physical.Replace('\', '\\'))) --mask ([regex]::Escape($physical))
    $exit = $LASTEXITCODE
    if ($Discover) {
        # As the scenario left the store: its customer (parity@example.com, the password it ends with), the administrator.
        & dotnet $verifier discover --url $url --out (Join-Path $PSScriptRoot 'regression\nop.discovered.iis.json') --inventory $inventory `
            --login-url /login.aspx --max-pages 1500 --persona anonymous --persona 'login:parity@example.com:Parity-1' --persona 'login:admin@yourstore.com:admin'
        if ($LASTEXITCODE -ne 0) { $exit = 1 }
    }
}
finally {
    Write-Host ("time: {0:mm\:ss}" -f $watch.Elapsed)
    if (-not $Keep) {
        Remove-Everything
        & (Join-Path $PSScriptRoot 'nop-database.ps1') -Drop
        if ($startedSql) { Stop-Service $sqlService }
    }
    else { Write-Host "kept: $url (.\corpora\record-nop-operations.ps1 -Remove)" }
}
exit $exit
