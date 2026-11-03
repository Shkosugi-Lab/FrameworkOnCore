# Experiment: build and run a converted app in a Linux container (mcr.microsoft.com/dotnet/sdk:10.0)
# and compare it, from the host, with the golden data recorded on IIS / .NET Framework.
#
#   .\experiments\wf4c\run-linux.ps1 -App ProductAdmin            # a sample (after run-sample.ps1)
#   .\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
#   .\experiments\wf4c\run-linux.ps1 -App wt\WingtipToys\WingtipToys -SqlServer -Suite operations
#       WingtipToys' operations suite (corpora\regression\wt.operations.json; the scenario wt.operations.scenario.json, the
#       golden data wt.operations.golden.json recorded on IIS by corpora\record-wt-operations.ps1): the database WingtipToys
#       dropped in the SQL Server container before the application starts (it creates and seeds it), the container's SQL
#       Server reached from here on 127.0.0.1:14333 for the scenario's state checks; per-operation results in
#       experiments\wf4c\_results\wt.operations.linux.{md,json}.
#   .\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Suite operations
#       BlogEngine.NET's operations suite (corpora\regression\be.operations.json; the scenario be.operations.scenario.json,
#       the golden data be.operations.golden.json recorded on IIS by corpora\record-be-operations.ps1): the release's
#       App_Data (corpora\be-app-data.ps1) copied into the container, the scenario's files steps reading the container's
#       (docker exec tar); per-operation results in experiments\wf4c\_results\be.operations.linux.{md,json}.
#   .\experiments\wf4c\run-linux.ps1 -App nop\NopCommerceStore -Suite operations
#       nopCommerce 1.90's operations suite (corpora\regression\nop.operations.json; the scenario nop.operations.scenario.json,
#       the golden data nop.operations.golden.json recorded on IIS by corpora\record-nop-operations.ps1): the conversion's
#       assembled site (nop\site: the web project with the payment, shipping, tax and promotion providers' assemblies, as
#       the conversion's Dockerfile deploys it) run as it is - not built again in the container -, file names without regard
#       to case (casefs, as the deployment's start.sh), the clients local (-LocalClient: nopCommerce keeps their address);
#       the database nopCommerceOps restored in the SQL Server container (corpora\nop-database.ps1) before the application
#       starts, ConnectionStrings.config pointing at it; per-operation results in experiments\wf4c\_results\nop.operations.linux.{md,json}.
#
# The app's tree (the first folder of -App, as convert-project.ps1 wrote it), _feed, shims and icu
# are copied into the container as they are (without build output), so the projects' relative
# paths to them hold there too.
#
# Exit code: ParityTest's (0: RESULT: OK, every snapshot the same or a documented difference for linux - the scenario's
# "allowed" file -, and every link; 1: RESULT: FAIL). A failed build or start-up throws (exit 1 under powershell -File).
param(
    # The web project's directory, relative to experiments\wf4c.
    [Parameter(Mandatory = $true)][string]$App,
    # Default: the sample's (samples\<App>\parity-scenario.json, golden-webforms.json).
    [string]$Scenario,
    [string]$Golden,
    [int]$Port = 5095,
    [string]$Image = 'mcr.microsoft.com/dotnet/sdk:10.0',
    # Run SQL Server in a container too (w2l-sql, kept between runs), and point web.config's
    # connection strings to a local instance (.\SQLEXPRESS, (LocalDB)\..., localhost) at it. Linux has
    # neither SQL Express nor LocalDB, nor Windows authentication.
    [switch]$SqlServer,
    # The process culture. IIS gives the app the server OS's culture (unless web.config's
    # <globalization> says otherwise); a container has none (invariant: '¤' for currency). Default:
    # this machine's, which is where the golden data was recorded.
    [string]$Culture = (Get-Culture).Name,
    # The process time zone (TZ, an IANA id). IIS runs the application in the server's time zone; a container without TZ
    # in UTC. Default: as the conversion's deployment (DeployWriter): the culture profile's TimeZone (App_Data\
    # culture-profile.json), else this machine's, converted with Windows' ICU (icu.dll).
    [string]$TimeZone,
    # The application seen from its own machine: it listens on the container's loopback, a proxy (socat) in the container
    # forwards the published port to it, so its clients' address is 127.0.0.1 (Request.IsLocal: customErrors RemoteOnly
    # shows ASP.NET's detailed errors, the client address the application keeps), as the golden data's IIS saw the
    # recorder on its own machine. Without it, Docker's port forwarding makes them the bridge's gateway (172.17.0.1).
    # On by default for be's and nop's operations suites.
    [switch]$LocalClient,
    # Leave the container running afterwards (docker exec into it; docker rm -f to remove).
    [switch]$Keep,
    # File names without regard to case, as on Windows (casefs/libfoccase.so, preloaded into the application's process,
    # for its folder). casefs\build.ps1 builds it.
    [switch]$CaseInsensitive,
    # More environment variables for the application (WEBFORMSFORCORE_PATH_CASING = '0', ...).
    [hashtable]$Environment = @{},
    # operations: the operations suite of WingtipToys (with -SqlServer), BlogEngine.NET or nopCommerce (above).
    [ValidateSet('default', 'operations')][string]$Suite = 'default'
)

# Not Stop: Windows PowerShell turns docker's stderr ("no such object") into terminating errors.
# Failures are checked by exit code.
$ErrorActionPreference = 'Continue'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$name = Split-Path $App -Leaf
# --platform linux: the documented differences of the scenario's "allowed" file that are Linux's apply.
$verifierArguments = @('--platform', 'linux')
$beOperations = $Suite -eq 'operations' -and $name -eq 'BlogEngine.NET'
$nopOperations = $Suite -eq 'operations' -and $name -eq 'NopCommerceStore'
# The folder run in the container: the web project's (built there), or the conversion's assembled site (run as it is).
$runFrom = $App
$prebuilt = $false
$sqlPassword = 'W2l_local_Passw0rd'   # a throwaway local container
if ($beOperations) {
    # BlogEngine.NET's operations suite: the release's App_Data (corpora\be-app-data.ps1) in the tree the container copies,
    # read by the scenario's files steps in the container (docker exec ... tar). The visitors' address is kept (the
    # comments' files) and its error pages are customErrors RemoteOnly: the client is the application's own machine.
    $LocalClient = $true
    $Scenario = 'corpora\regression\be.operations.scenario.json'
    $Golden = 'corpora\parity\be.operations.golden.json'
    $results = Join-Path $PSScriptRoot '_results'
    New-Item -ItemType Directory $results -Force | Out-Null
    & (Join-Path $repo 'corpora\be-app-data.ps1') -Target (Join-Path $PSScriptRoot "$App\App_Data")
    $verifierArguments += @('--files', "docker:w2l-$($name.ToLowerInvariant() -replace '[^a-z0-9]', '-'):/work/$($App.Replace('\', '/'))/App_Data",
        # The application's folder (its tools page, its blogs' storage, the error pages name it).
        '--mask', [regex]::Escape("/work/$($App.Replace('\', '/'))"),
        '--report', (Join-Path $results 'be.operations.linux.md'), '--results', (Join-Path $results 'be.operations.linux.json'))
}
elseif ($nopOperations) {
    # nopCommerce's operations suite: the assembled site (its bin has the providers' assemblies the web project's build
    # alone does not put there), SQL Server, the clients' address local (the online customers, the customers' last IP
    # address), file names without regard to case (the deployment's default: start.sh preloads casefs).
    $runFrom = Join-Path (Split-Path $App -Parent) 'site'
    $prebuilt = $true
    $SqlServer = $true
    $LocalClient = $true
    if (Test-Path (Join-Path $PSScriptRoot 'casefs\out\linux-x64\libfoccase.so')) { $CaseInsensitive = $true }
    # (Another scenario and golden data may be given: to try a part of the suite out.)
    if (-not $Scenario) { $Scenario = 'corpora\regression\nop.operations.scenario.json' }
    if (-not $Golden) { $Golden = 'corpora\parity\nop.operations.golden.json' }
    $results = Join-Path $PSScriptRoot '_results'
    New-Item -ItemType Directory $results -Force | Out-Null
    $verifierArguments += @('--sql', "Server=127.0.0.1,14333;Database=nopCommerceOps;User ID=sa;Password=$sqlPassword;TrustServerCertificate=true",
        # The application's folder (the error pages and the system information name it).
        '--mask', [regex]::Escape("/work/$($runFrom.Replace('\', '/'))"),
        '--report', (Join-Path $results 'nop.operations.linux.md'), '--results', (Join-Path $results 'nop.operations.linux.json'))
}
elseif ($Suite -eq 'operations') {
    if (-not $SqlServer) { throw '-Suite operations needs -SqlServer' }
    $Scenario = 'corpora\regression\wt.operations.scenario.json'
    $Golden = 'corpora\parity\wt.operations.golden.json'
    $results = Join-Path $PSScriptRoot '_results'
    New-Item -ItemType Directory $results -Force | Out-Null
    $verifierArguments += @('--sql', 'Server=127.0.0.1,14333;Database=WingtipToys;User ID=sa;Password=W2l_local_Passw0rd;TrustServerCertificate=true',
        '--report', (Join-Path $results 'wt.operations.linux.md'), '--results', (Join-Path $results 'wt.operations.linux.json'))
}
if (-not $Scenario) { $Scenario = "samples\$name\parity-scenario.json" }
if (-not $Golden) { $Golden = "samples\$name\golden-webforms.json" }
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
# The parity verifier (Playwright), built once into bin\alt (a separate output: a running copy does not lock the build).
& {  # built every time (incremental: quick): the verifier run is the one of this checkout
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
}
$appPath = $runFrom.Replace('\', '/')
$container = "w2l-$($name.ToLowerInvariant() -replace '[^a-z0-9]', '-')"
$logDir = Join-Path $PSScriptRoot "_linux\$name"
New-Item -ItemType Directory $logDir -Force | Out-Null

# Files laid over the copied app in the container (deployment-time configuration; the app's own
# files stay as they are).
$overlay = Join-Path $logDir 'overlay'
if (Test-Path $overlay) { Remove-Item $overlay -Recurse -Force }
New-Item -ItemType Directory $overlay -Force | Out-Null
$network = @()
if ($SqlServer) {
    docker network create w2l 2>$null | Out-Null
    # Published on this machine's loopback (127.0.0.1:14333): the operations suite's state checks read the database.
    if ((docker inspect -f '{{.State.Running}}' w2l-sql 2>$null) -ne 'true' -or -not (docker port w2l-sql 1433 2>$null)) {
        docker rm -f w2l-sql 2>$null | Out-Null
        docker run -d --name w2l-sql --network w2l -p 127.0.0.1:14333:1433 -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$sqlPassword" mcr.microsoft.com/mssql/server:2022-latest | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "could not start SQL Server" }
    }
    foreach ($attempt in 1..60) {
        docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -Q 'SELECT 1' 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { break }
        Start-Sleep -Seconds 2
    }
    $network = @('--network', 'w2l')
    if ($nopOperations) {
        # The known state: the sample store's database (as on IIS: record-nop-operations.ps1), and the connection string
        # as nopCommerce's installer writes it (ConnectionStrings.config), to the SQL Server container.
        & powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repo 'corpora\nop-database.ps1') -Server '127.0.0.1,14333' -User sa -Password $sqlPassword
        if ($LASTEXITCODE -ne 0) { throw 'could not restore the nop database' }
        [IO.File]::WriteAllText((Join-Path $overlay 'ConnectionStrings.config'), "<connectionStrings>`r`n  <add name=`"NopSqlConnection`" connectionString=`"Data Source=w2l-sql;Initial Catalog=nopCommerceOps;User ID=sa;Password=$sqlPassword;TrustServerCertificate=True;MultipleActiveResultSets=True;Connect Timeout=120`" />`r`n</connectionStrings>")
    }
    elseif ($Suite -eq 'operations') {
        # The known state: no database; the application's start creates and seeds it (as on IIS: record-wt-operations.ps1).
        docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -b -Q "IF DB_ID(N'WingtipToys') IS NOT NULL BEGIN ALTER DATABASE [WingtipToys] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [WingtipToys]; END" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'could not drop WingtipToys' }
    }

    $webConfigFile = Get-ChildItem (Join-Path $PSScriptRoot $runFrom) -Filter 'web.config' | Select-Object -First 1
    $webConfig = New-Object Xml
    $webConfig.PreserveWhitespace = $true
    $webConfig.Load($webConfigFile.FullName)
    foreach ($add in $webConfig.SelectNodes('/configuration/connectionStrings/add')) {
        $builder = New-Object Data.Common.DbConnectionStringBuilder
        # set_ConnectionString: in PowerShell, "$builder.ConnectionString = ..." adds a key of that name.
        try { $builder.set_ConnectionString($add.connectionString) } catch { continue }
        $server = @('Data Source', 'Server', 'Address', 'Addr') | Where-Object { $builder.ContainsKey($_) } | ForEach-Object { $builder.get_Item($_) } | Select-Object -First 1
        if (-not $server -or $server -notmatch '^(\.|\(local\)|localhost|127\.0\.0\.1|\(LocalDB\))(\\|$)') { continue }
        $database = @('Initial Catalog', 'Database') | Where-Object { $builder.ContainsKey($_) } | ForEach-Object { $builder.get_Item($_) } | Select-Object -First 1
        if (-not $database -and $builder.ContainsKey('AttachDbFilename')) { $database = [IO.Path]::GetFileNameWithoutExtension($builder.get_Item('AttachDbFilename')) }
        $new = "Data Source=w2l-sql;Initial Catalog=$database;User ID=sa;Password=$sqlPassword;TrustServerCertificate=True"
        if ($builder.ContainsKey('MultipleActiveResultSets')) { $new += ";MultipleActiveResultSets=$($builder.get_Item('MultipleActiveResultSets'))" }
        Write-Host "connection string '$($add.name)': $($add.connectionString) -> $new"
        $add.connectionString = $new
    }
    $webConfig.Save((Join-Path $overlay $webConfigFile.Name))
}

if (-not $TimeZone) {
    $windowsZone = [TimeZoneInfo]::Local.Id
    $profileFile = Join-Path $PSScriptRoot "$runFrom\App_Data\culture-profile.json"
    if (Test-Path $profileFile) {
        $zone = (Get-Content $profileFile -Raw -Encoding UTF8 | ConvertFrom-Json).TimeZone
        if ($zone) { $windowsZone = $zone }
    }
    # Windows' ICU (Windows 10 1903 and later) maps Windows' ids to IANA's (as .NET's TimeZoneInfo.TryConvertWindowsIdToIanaId).
    if (-not ('FocWindowsIcu' -as [type])) {
        Add-Type -TypeDefinition @'
using System.Runtime.InteropServices;
using System.Text;
public static class FocWindowsIcu {
    [DllImport("icu.dll", CharSet = CharSet.Unicode)]
    static extern int ucal_getTimeZoneIDForWindowsID(string winid, int len, [MarshalAs(UnmanagedType.LPStr)] string region, StringBuilder id, int capacity, ref int status);
    public static string Iana(string windowsId) {
        var id = new StringBuilder(128);
        int status = 0;
        int length = ucal_getTimeZoneIDForWindowsID(windowsId, windowsId.Length, null, id, id.Capacity, ref status);
        return status > 0 || length <= 0 ? null : id.ToString(0, length);
    }
}
'@
    }
    $TimeZone = [FocWindowsIcu]::Iana($windowsZone)
    if (-not $TimeZone) { Write-Warning "time zone $windowsZone has no IANA id: UTC"; $TimeZone = 'UTC' }
}
Write-Host "time zone: $TimeZone"
$appPort = if ($LocalClient) { $Port + 1 } else { $Port }

# FrameworkOnCore's packages keep one version (1.6.5-w2l.x) across rebuilds; the container's package cache
# (a volume, kept between runs) must not serve an older build of it.
$script = @"
set -e
# System.Drawing on Linux (FrameworkOnCore's System.Drawing.Common) draws with SkiaSharp: fontconfig and fonts, as the deployment installs them.
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libfontconfig1 fonts-liberation2 $(if ($LocalClient) { 'socat ' })>/dev/null 2>&1 || true
rm -rf /root/.nuget/packages/frameworkoncore.*
mkdir -p /work
$(if ($prebuilt) { "cd /src && tar -cf - icu $appPath | (cd /work && tar xf -)" } else { "cd /src && tar --exclude='*/bin' --exclude='*/obj' -cf - _feed shims icu $($appPath.Split('/')[0]) | (cd /work && tar xf -)" })
cd /work/$appPath
cp -r /overlay/. .
# The original server's culture data (capture-culture.ps1, put in App_Data by the conversion) made
# ICU's: new CultureInfo("ja-JP") and every other way of creating a culture gets them.
if [ -f App_Data/culture-profile.json ]; then
    bash /work/icu/build-icu-data.sh App_Data/culture-profile.json /icu-data
    export ICU_DATA=/icu-data
fi
$(if (-not $prebuilt) { 'dotnet build -v q -nologo' })
# The build servers (MSBuild, the compiler: about 1 GB) are not needed by the app: with SQL Server in the same
# Docker VM, they left the app too little memory (killed on its first request).
dotnet build-server shutdown >/dev/null 2>&1 || true
# What the conversion did to bin after its build (the converted bin is the one deployed; the build here makes a new one):
# frameworkoncore.*.txt (the platform rules for what the server compiles - App_Code, the pages, Web Pages -, the culture
# names), and the translations kept for the original server's languages (LanguagePacks: the others removed, Microsoft's
# language packs' made). As in the converted bin.
# (No double quotes in this script: Windows PowerShell's passing of the argument to docker would split it at them.)
if [ -d /src/$appPath/bin ] && [ $(if ($prebuilt) { 'yes' } else { 'no' }) = no ]; then
    shopt -s nullglob
    cp /src/$appPath/bin/frameworkoncore.*.txt bin/ 2>/dev/null || true
    for f in bin/*/*.resources.dll; do [ -e /src/$appPath/`$f ] || { rm -f `$f; echo not in the converted bin, removed: `$f; }; done
    (cd /src/$appPath && for f in bin/*/*.resources.dll; do [ -e /work/$appPath/`$f ] || cp `$f /work/$appPath/`$f; done)
    shopt -u nullglob
fi
$(if ($CaseInsensitive) { "export LD_PRELOAD=/foccase/linux-`$(uname -m | sed s/x86_64/x64/\;s/aarch64/arm64/)/libfoccase.so FOC_CASE_ROOTS=/work/$appPath FOC_CASE_LOG=1" })
$(if ($LocalClient) { "command -v socat >/dev/null || { echo 'socat not installed (-LocalClient)'; exit 1; }`nsocat TCP-LISTEN:$Port,fork,reuseaddr TCP:127.0.0.1:$appPort &`nexec dotnet bin/$name.dll --urls http://127.0.0.1:$appPort" } else { "exec dotnet bin/$name.dll --urls http://0.0.0.0:$Port" })
"@ -replace "`r", ''

docker rm -f $container 2>$null | Out-Null
$caseArguments = @()
if ($CaseInsensitive) { $caseArguments = '-v', "$(Join-Path $PSScriptRoot 'casefs\out'):/foccase:ro" }
$caseArguments += @($Environment.Keys | ForEach-Object { '-e'; "$_=$($Environment[$_])" })
# The golden data was recorded from IIS's own machine (a local client: IIS's detailed error pages, the application's
# error answers left): this machine taken as the container's (HttpErrors:LocalAddresses).
# A fixed host name (the application shows it: ELMAH's log), masked in the comparison.
$verifierArguments += @('--mask', $container)
docker run -d --name $container --hostname $container @network @caseArguments -e 'HttpErrors__LocalAddresses=*' -e "LANG=$($Culture.Replace('-', '_')).UTF-8" -e "TZ=$TimeZone" -p "${Port}:${Port}" -v "${PSScriptRoot}:/src:ro" -v "${overlay}:/overlay:ro" -v w2l-nuget:/root/.nuget/packages $Image bash -c $script | Out-Null
if ($LASTEXITCODE -ne 0) { throw "docker run failed" }
try {
    # Build (restore included) and start-up: wait until the app answers, or the container stops.
    $ready = $false
    foreach ($attempt in 1..300) {
        if ((docker inspect -f '{{.State.Running}}' $container) -ne 'true') { break }
        try { Invoke-WebRequest "http://localhost:$Port/" -UseBasicParsing -TimeoutSec 120 | Out-Null; $ready = $true; break }
        catch { if ($_.Exception.Response) { $ready = $true; break }; Start-Sleep -Seconds 2 }
    }
    if (-not $ready) {
        docker logs $container 2>&1 | Select-Object -Last 40
        throw "the app did not start in the container (log above)"
    }
    $watch = [Diagnostics.Stopwatch]::StartNew()
    & dotnet $verifier verify --url "http://localhost:$Port" --scenario (Join-Path $repo $Scenario) --golden (Join-Path $repo $Golden) @verifierArguments
    $verifyExit = $LASTEXITCODE
    Write-Host ("time: {0:mm\:ss}" -f $watch.Elapsed)
}
finally {
    docker logs $container *> (Join-Path $logDir 'container.log')
    if (-not $Keep) { docker rm -f $container 2>$null | Out-Null }
}
exit $verifyExit
