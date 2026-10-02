# Experiment: build and run a converted app in a Linux container (mcr.microsoft.com/dotnet/sdk:10.0)
# and compare it, from the host, with the golden data recorded on IIS / .NET Framework.
#
#   .\experiments\wf4c\run-linux.ps1 -App ProductAdmin            # a sample (after run-sample.ps1)
#   .\experiments\wf4c\run-linux.ps1 -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
#
# The app's tree (the first folder of -App, as convert-project.ps1 wrote it), _feed, shims and icu
# are copied into the container as they are (without build output), so the projects' relative
# paths to them hold there too.
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
    # Leave the container running afterwards (docker exec into it; docker rm -f to remove).
    [switch]$Keep,
    # File names without regard to case, as on Windows (casefs/libfoccase.so, preloaded into the application's process,
    # for its folder). casefs\build.ps1 builds it.
    [switch]$CaseInsensitive,
    # More environment variables for the application (WEBFORMSFORCORE_PATH_CASING = '0', ...).
    [hashtable]$Environment = @{}
)

# Not Stop: Windows PowerShell turns docker's stderr ("no such object") into terminating errors.
# Failures are checked by exit code.
$ErrorActionPreference = 'Continue'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$name = Split-Path $App -Leaf
if (-not $Scenario) { $Scenario = "samples\$name\parity-scenario.json" }
if (-not $Golden) { $Golden = "samples\$name\golden-webforms.json" }
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
# The parity verifier (Playwright), built once into bin\alt (a separate output: a running copy does not lock the build).
& {  # built every time (incremental: quick): the verifier run is the one of this checkout
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
}
$appPath = $App.Replace('\', '/')
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
    $sqlPassword = 'W2l_local_Passw0rd'   # a throwaway local container
    docker network create w2l 2>$null | Out-Null
    if ((docker inspect -f '{{.State.Running}}' w2l-sql 2>$null) -ne 'true') {
        docker rm -f w2l-sql 2>$null | Out-Null
        docker run -d --name w2l-sql --network w2l -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$sqlPassword" mcr.microsoft.com/mssql/server:2022-latest | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "could not start SQL Server" }
    }
    foreach ($attempt in 1..60) {
        docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -Q 'SELECT 1' 2>$null | Out-Null
        if ($LASTEXITCODE -eq 0) { break }
        Start-Sleep -Seconds 2
    }
    $network = @('--network', 'w2l')

    $webConfigFile = Get-ChildItem (Join-Path $PSScriptRoot $App) -Filter 'web.config' | Select-Object -First 1
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

# FrameworkOnCore's packages keep one version (1.6.5-w2l.x) across rebuilds; the container's package cache
# (a volume, kept between runs) must not serve an older build of it.
$script = @"
set -e
# System.Drawing's Linux implementation (FrameworkOnCore's System.Drawing.Common) draws with libgdiplus, as the deployment installs it.
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 >/dev/null 2>&1 || true
rm -rf /root/.nuget/packages/frameworkoncore.*
mkdir -p /work
cd /src && tar --exclude='*/bin' --exclude='*/obj' -cf - _feed shims icu $($appPath.Split('/')[0]) | (cd /work && tar xf -)
cd /work/$appPath
cp -r /overlay/. .
# The original server's culture data (capture-culture.ps1, put in App_Data by the conversion) made
# ICU's: new CultureInfo("ja-JP") and every other way of creating a culture gets them.
if [ -f App_Data/culture-profile.json ]; then
    bash /work/icu/build-icu-data.sh App_Data/culture-profile.json /icu-data
    export ICU_DATA=/icu-data
fi
dotnet build -v q -nologo
# The build servers (MSBuild, the compiler: about 1 GB) are not needed by the app: with SQL Server in the same
# Docker VM, they left the app too little memory (killed on its first request).
dotnet build-server shutdown >/dev/null 2>&1 || true
$(if ($CaseInsensitive) { "export LD_PRELOAD=/foccase/linux-`$(uname -m | sed s/x86_64/x64/\;s/aarch64/arm64/)/libfoccase.so FOC_CASE_ROOTS=/work/$appPath FOC_CASE_LOG=1" })
exec dotnet bin/$name.dll --urls http://0.0.0.0:$Port
"@ -replace "`r", ''

docker rm -f $container 2>$null | Out-Null
$caseArguments = @()
if ($CaseInsensitive) { $caseArguments = '-v', "$(Join-Path $PSScriptRoot 'casefs\out'):/foccase:ro" }
$caseArguments += @($Environment.Keys | ForEach-Object { '-e'; "$_=$($Environment[$_])" })
docker run -d --name $container @network @caseArguments -e "LANG=$($Culture.Replace('-', '_')).UTF-8" -p "${Port}:${Port}" -v "${PSScriptRoot}:/src:ro" -v "${overlay}:/overlay:ro" -v w2l-nuget:/root/.nuget/packages $Image bash -c $script | Out-Null
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
    & dotnet $verifier verify --url "http://localhost:$Port" --scenario (Join-Path $repo $Scenario) --golden (Join-Path $repo $Golden)
}
finally {
    docker logs $container *> (Join-Path $logDir 'container.log')
    if (-not $Keep) { docker rm -f $container 2>$null | Out-Null }
}
