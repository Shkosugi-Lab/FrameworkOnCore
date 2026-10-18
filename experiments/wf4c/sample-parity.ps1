# A sample (samples\<Name>) as its users see it, against the original on IIS: -Record takes the golden data from the
# sample built for .NET Framework 4.8 and run on IIS (FrameworkOnCore.ParityTest record, the sample's
# parity-scenario.json, into samples\<Name>\golden-webforms.json); -Windows and -Linux convert the sample with the
# converter (its default choices), run it (on Windows from its bin, on Linux from the Dockerfile the converter writes)
# and compare (ParityTest verify). As corpora\record-webforms-golden.ps1 and run-linux.ps1 do for the corpora, for a
# sample whose packages run-sample.ps1's template does not have (System.Web.Mobile's, the charts', the Ajax Control
# Toolkit's).
#
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Record      # IIS (an administrator's shell)
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Record -LinksOnly   # its links (crawl) only, its snapshots kept
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Windows
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Linux       # Docker
#   .\experiments\wf4c\sample-parity.ps1 -Name BclProbe -Windows -Diff   # also recorded: sample-diff.ps1 -Recorded shows every line that differs
#
# A sample with samples\<Name>\database.json ({ "name": "<database>", "connection": "<connectionStrings name>",
# "regsql": "mrp" }) gets its database made afresh on SQL Server Express (.\SQLEXPRESS) before each run, with
# aspnet_regsql's schema (-A: m membership, r roles, p profile): the same state for the original and the converted
# application. IIS's pool is given it; the container reaches it through host.docker.internal,1433 with the SQL login of
# _verify\sql-login.txt (as studio-verify.ps1), as SQLCONNSTR_<connection>.
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [switch]$Record,
    [switch]$LinksOnly,
    [switch]$Windows,
    [switch]$Linux,
    # Also record what the converted application showed (_samples\<Name>\recorded-windows.json, recorded-linux.json).
    [switch]$Diff,
    [int]$Port = 5096,
    # The original server's IIS settings for the site (capture-iis.ps1), given to the converter (--iis-profile).
    [string]$IisProfile
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$sample = Join-Path $repo "samples\$Name"
$scenario = Join-Path $sample 'parity-scenario.json'
$golden = Join-Path $sample 'golden-webforms.json'
$work = Join-Path $PSScriptRoot "_samples\$Name"
New-Item -ItemType Directory $work -Force | Out-Null
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
& {  # built every time (incremental: quick): the verifier run is the one of this checkout
    dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'FrameworkOnCore.ParityTest build failed' }
}

function Wait-Site([string]$base) {
    foreach ($attempt in 1..90) {
        try { Invoke-WebRequest "$base/" -UseBasicParsing -TimeoutSec 60 | Out-Null; return }
        catch { if ($_.Exception.Response) { return }; Start-Sleep -Seconds 2 }
    }
    throw "$base did not answer"
}

$databaseFile = Join-Path $sample 'database.json'
$database = if (Test-Path $databaseFile) { Get-Content $databaseFile -Raw | ConvertFrom-Json } else { $null }
function Reset-Database([string]$login) {
    if (-not $database) { return }
    $name = $database.name
    sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID('$name') IS NOT NULL BEGIN ALTER DATABASE [$name] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$name] END; CREATE DATABASE [$name]" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw "the database $name was not made" }
    if ($database.regsql) {
        $regsql = Join-Path $env:SystemRoot 'Microsoft.NET\Framework64\v4.0.30319\aspnet_regsql.exe'
        & $regsql -S .\SQLEXPRESS -E -d $name -A $database.regsql -Q | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "aspnet_regsql failed on $name" }
    }
    if ($login) {
        sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF SUSER_ID('$login') IS NULL CREATE LOGIN [$login] FROM WINDOWS; USE [$name]; CREATE USER [$login] FOR LOGIN [$login]; ALTER ROLE db_owner ADD MEMBER [$login]" | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "$login was not given $name" }
    }
}

# A file just closed may still be held a moment (the virus scanner): tried again.
function Remove-Folder([string]$folder) {
    foreach ($attempt in 1..10) {
        if (-not (Test-Path $folder)) { return }
        try { [IO.Directory]::Delete($folder, $true) } catch { if ($attempt -eq 10) { throw }; Start-Sleep -Seconds 3 }
    }
}

if ($Record) {
    $appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
    $site = Join-Path $work 'iis'
    Remove-Folder $site
    Copy-Item $sample $site -Recurse
    # The packages of packages.config, from nuget.org into the packages folder next to the site (the HintPaths'
    # ..\packages\<id>.<version>): .NET Framework's MSBuild does not restore them.
    $packagesConfig = Join-Path $sample 'packages.config'
    if (Test-Path $packagesConfig) {
        Add-Type -AssemblyName System.IO.Compression.FileSystem
        foreach ($package in ([xml](Get-Content $packagesConfig -Raw)).packages.package) {
            $folder = Join-Path $work "packages\$($package.id).$($package.version)"
            if (Test-Path $folder) { continue }
            $nupkg = Join-Path $work "$($package.id).$($package.version).nupkg"
            $lower = $package.id.ToLowerInvariant()
            Invoke-WebRequest "https://api.nuget.org/v3-flatcontainer/$lower/$($package.version)/$lower.$($package.version).nupkg" -OutFile $nupkg -UseBasicParsing
            [IO.Compression.ZipFile]::ExtractToDirectory($nupkg, $folder)
        }
    }
    # Visual Studio's MSBuild when there is one (its compiler has the C# a .NET Framework 4.8 project is written in: the
    # older samples' are C# 6 and later), else .NET Framework's own (C# 5).
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $msbuild = if (Test-Path $vswhere) { & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1 }
    if (-not $msbuild) { $msbuild = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' }
    & $msbuild (Join-Path $site "$Name.csproj") /nologo /v:q /p:Configuration=Debug
    if ($LASTEXITCODE -ne 0) { throw 'the sample did not build for .NET Framework' }
    $pool = "frameworkoncore-$($Name.ToLowerInvariant())"
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
    & $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    Reset-Database "IIS AppPool\$pool"
    & $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:localhost" -f $Port) | Out-Null
    & $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
    & $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
    # The pool's identity reads the site (IIS answers 500.19 otherwise) and writes App_Data.
    & icacls $site /grant "IIS AppPool\${pool}:(OI)(CI)(M)" /T /Q | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier $(if ($LinksOnly) { 'record-links' } else { 'record' }) --url "http://localhost:$Port" --scenario $scenario --out $golden
        if ($LASTEXITCODE -ne 0) { throw 'recording failed' }
        Write-Host "-> $golden (look at it: an error page recorded is what every run is compared with)" -ForegroundColor Yellow
    }
    finally {
        & $appcmd delete site $pool 2>&1 | Out-Null
        & $appcmd delete apppool $pool 2>&1 | Out-Null
    }
    exit 0
}

# Converted with the converter's default choices, built, with a Dockerfile (for -Linux).
$converted = Join-Path $work 'converted'
Remove-Folder $converted
$converter = Join-Path $repo 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
dotnet build (Join-Path $repo 'src\FrameworkOnCore.Converter') --nologo -v q | Out-Null
# A sample with samples\<Name>\cultures.txt (culture names, comma-separated): their data captured here, where IIS runs it
# (capture-culture.ps1, Windows PowerShell), given to the converter (--culture-profile): Linux's ICU data made the same.
$cultureArguments = @()
$culturesFile = Join-Path $sample 'cultures.txt'
if (Test-Path $culturesFile) {
    $cultureProfile = Join-Path $work 'culture-profile.json'
    $cultures = @((Get-Content $culturesFile -Raw).Split(@(',', "`r", "`n"), [StringSplitOptions]::RemoveEmptyEntries) | ForEach-Object { $_.Trim() } | Where-Object { $_ })
    $capture = Join-Path $PSScriptRoot 'capture-culture.ps1'
    powershell.exe -NoProfile -Command "& '$capture' -Out '$cultureProfile' -WebConfig '$(Join-Path $sample 'Web.config')' -Cultures @('$($cultures -join "','")')" | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'capture-culture.ps1 failed' }
    $cultureArguments = @('--culture-profile', $cultureProfile)
}
if ($IisProfile) { $cultureArguments += @('--iis-profile', (Resolve-Path $IisProfile).Path) }
& dotnet $converter (Join-Path $sample "$Name.csproj") --out $converted --root $sample --deploy container @cultureArguments
if ($LASTEXITCODE -ne 0) { throw 'the conversion failed' }

$exit = 0
if ($Windows) {
    Reset-Database
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'windows-out.log') -RedirectStandardError (Join-Path $work 'windows-err.log')
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier verify --url "http://localhost:$Port" --scenario $scenario --golden $golden
        $exit = $LASTEXITCODE
        # -Diff: what it showed, recorded (a second run of the scenario) for sample-diff.ps1 -Recorded: every line that differs.
        if ($Diff) { & dotnet $verifier record --url "http://localhost:$Port" --scenario $scenario --out (Join-Path $work 'recorded-windows.json') | Out-Null }
    }
    # The tree: the application runs in a worker process its process starts (FrameworkOnCore's System.Web).
    # Ended by itself (an exception at its start): its error shown, not taskkill's on a process no longer there.
    finally {
        if ($process.HasExited) { Get-Content (Join-Path $work 'windows-err.log') -Tail 20 | Write-Host -ForegroundColor Red }
        else { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
    }
}

if ($Linux) {
    $ErrorActionPreference = 'Continue'   # docker's stderr is not an error; exit codes are checked
    $image = "w2l-$($Name.ToLowerInvariant())"
    docker build -t $image $converted *> (Join-Path $work 'docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed (see $work\docker-build.log)" }
    cmd /c "docker rm -f $image" 2>&1 | Out-Null
    # The sample's settings for its container (samples\<Name>\linux.env: what differs there, the ports behind the mapping).
    $envFile = Join-Path $sample 'linux.env'
    $envArguments = if (Test-Path $envFile) { @('--env-file', $envFile) } else { @() }
    if ($database) {
        Reset-Database
        $login = (Get-Content (Join-Path $PSScriptRoot '_verify\sql-login.txt') -Raw).Trim()
        $envArguments += @('-e', "SQLCONNSTR_$($database.connection)=Server=host.docker.internal,1433;Database=$($database.name);$login;TrustServerCertificate=True")
    }
    # The golden data was recorded from IIS's own machine (a local client: IIS's detailed error pages, the application's
    # error answers left): this machine taken as the container's (HttpErrors:LocalAddresses).
    docker run -d --name $image -p "${Port}:8080" -e 'HttpErrors__LocalAddresses=*' @envArguments $image | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier verify --url "http://localhost:$Port" --scenario $scenario --golden $golden
        $exit = $LASTEXITCODE
        # -Diff: what it showed, recorded (a second run of the scenario) for sample-diff.ps1 -Recorded: every line that differs.
        if ($Diff) { & dotnet $verifier record --url "http://localhost:$Port" --scenario $scenario --out (Join-Path $work 'recorded-linux.json') | Out-Null }
    }
    finally {
        docker logs $image *> (Join-Path $work 'linux-server.log')
        cmd /c "docker rm -f $image" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
exit $exit
