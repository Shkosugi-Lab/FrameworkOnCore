# The component parity of System.Web's controls (README.md): a component's three layers, recorded from .NET Framework
# (the goldens) or run on the port and compared, into Markdown (reports\<Component>\).
#
#   .\tests\WebControlsParity\parity.ps1 -Control TextBox -Record     # .NET Framework 4.8 and IIS (an administrator's shell)
#   .\tests\WebControlsParity\parity.ps1 -Control TextBox -Windows    # the port on Windows
#   .\tests\WebControlsParity\parity.ps1 -Control TextBox -Linux      # the port on Linux (Docker)
#   .\tests\WebControlsParity\parity.ps1 -Control Table,TableRow -Windows   # several: the site built or converted once
#
# -Record: the API layer under net48 (golden\<Component>\api.golden.txt, cases.golden.json); the probe site
#   (samples\WebControlsProbe) built for .NET Framework 4.8 and run on IIS, its rendering cases fetched
#   (render.golden.json, render.golden-cases.tsv) and its scenario recorded (scenarios\<Component>.json:
#   scenario.golden.json); then the test cases' report (reports\<Component>\cases.md).
# -Windows / -Linux: the API layer under net10.0 against FrameworkOnCore's System.Web (on Linux in a container); the
#   probe site converted by the converter and run (on Windows from its bin, on Linux from the Dockerfile the converter
#   writes), its rendering cases fetched and its scenario verified; then the results (reports\<Component>\results-<platform>.md).
# One thing at a time: the site, then the next (CLAUDE.md: containers one by one).
param(
    [string[]]$Control = @('TextBox'),
    [switch]$Record,
    [switch]$Windows,
    [switch]$Linux,
    [int]$Port = 5097
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$here = $PSScriptRoot
$Control = @($Control | ForEach-Object { $_ -split ',' } | Where-Object { $_ })
$sample = Join-Path $repo 'samples\WebControlsProbe'
$work = Join-Path $repo 'experiments\wf4c\_samples\WebControlsProbe'
New-Item -ItemType Directory $work -Force | Out-Null
# The component being run (Use-Component): its goldens, reports, scenario.
function Use-Component([string]$name) {
    $script:Current = $name
    $script:golden = Join-Path $here "golden\$name"
    $script:reports = Join-Path $here "reports\$name"
    $script:scenario = Join-Path $sample "scenarios\$name.json"
    New-Item -ItemType Directory $script:golden, $script:reports -Force | Out-Null
}
$program = Join-Path $here 'bin\Debug\net10.0\WebControlsParity.dll'
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'

function Invoke-Checked([string]$what, [scriptblock]$command) {
    & $command
    if ($LASTEXITCODE -ne 0) { throw "$what failed ($LASTEXITCODE)" }
}

function Wait-Site([string]$base) {
    foreach ($attempt in 1..90) {
        try { Invoke-WebRequest "$base/Cases.aspx?component=$Current" -UseBasicParsing -TimeoutSec 60 | Out-Null; return }
        catch { if ($_.Exception.Response) { throw "$base answered $([int]$_.Exception.Response.StatusCode)" }; Start-Sleep -Seconds 2 }
    }
    throw "$base did not answer"
}

# The site's pages answer (a page that does not parse is a 500 the comparisons would only show as a difference).
function Test-Pages([string]$base) {
    # The scenario's page: the first it opens (the list controls share one).
    $scenarioPage = ((Get-Content $scenario -Raw | ConvertFrom-Json).steps | Where-Object { $_.action -eq 'goto' } | Select-Object -First 1).path.TrimStart('/')
    foreach ($page in @("Cases.aspx?component=$Current", "RenderProbe.aspx?component=$Current&case=$([uri]::EscapeDataString("$Current.(default)"))", $scenarioPage)) {
        try { Invoke-WebRequest "$base/$page" -UseBasicParsing -TimeoutSec 120 | Out-Null }
        catch { throw "$base/$page answered $(if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { $_.Exception.Message }): $($_.ErrorDetails.Message | Select-String 'Error Message|Exception Details' | Select-Object -First 2)" }
    }
}

# A file just closed may still be held a moment (the virus scanner): tried again.
function Remove-Folder([string]$folder) {
    foreach ($attempt in 1..10) {
        if (-not (Test-Path $folder)) { return }
        try { [IO.Directory]::Delete($folder, $true) } catch { if ($attempt -eq 10) { throw }; Start-Sleep -Seconds 3 }
    }
}

# Both of this checkout: the parity program (net48 for the goldens, net10.0 against the port and for the comparisons)
# and the scenario tool.
Invoke-Checked 'the parity program build' { dotnet build $here -nologo -v q | Out-Null }
Invoke-Checked 'the scenario tool build' { dotnet build (Join-Path $repo 'tools\FrameworkOnCore.ParityTest') -o (Split-Path $verifier) --nologo -v q | Out-Null }

# The layers that run against a site: the rendering cases fetched, the scenario recorded or verified.
function Invoke-SiteLayers([string]$base, [string]$out, [switch]$Recording) {
    Test-Pages $base
    if ($Recording) {
        Invoke-Checked 'the rendering layer' { dotnet $program render --control $Current --url $base --out (Join-Path $golden 'render.golden.json') }
        Invoke-Checked 'the scenario layer' { dotnet $verifier record --url $base --scenario $scenario --out (Join-Path $golden 'scenario.golden.json') }
    }
    else {
        Invoke-Checked 'the rendering layer' { dotnet $program render --control $Current --url $base --out (Join-Path $out 'render.json') --golden (Join-Path $golden 'render.golden.json') }
        # A difference is the report's to show: the exit code is not checked, nor what the tool writes to stderr (a step
        # that fails on the port, a control its page does not have: the report shows the scenario as not run).
        & {
            $ErrorActionPreference = 'Continue'
            dotnet $verifier verify --url $base --scenario $scenario --golden (Join-Path $golden 'scenario.golden.json') --report (Join-Path $out 'scenario-report.md') 2>&1 | ForEach-Object { "$_" }
        }
    }
}

if ($Record) {
    # The API layer on .NET Framework.
    foreach ($name in $Control) {
        Use-Component $name
        $apiOut = Join-Path $work 'golden-api'
        Invoke-Checked 'the API layer' { & (Join-Path $here 'bin\Debug\net48\WebControlsParity.exe') api --control $Current --out $apiOut }
        Copy-Item (Join-Path $apiOut 'api.txt') (Join-Path $golden 'api.golden.txt') -Force
        Copy-Item (Join-Path $apiOut 'cases.json') (Join-Path $golden 'cases.golden.json') -Force
    }

    # The probe site on IIS, built by Visual Studio's MSBuild (or .NET Framework's own).
    $appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
    $site = Join-Path $work 'iis'
    Remove-Folder $site
    Copy-Item $sample $site -Recurse
    $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
    $msbuild = if (Test-Path $vswhere) { & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1 }
    if (-not $msbuild) { $msbuild = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' }
    Invoke-Checked 'the .NET Framework build of the probe site' { & $msbuild (Join-Path $site 'WebControlsProbe.csproj') /nologo /v:q /p:Configuration=Debug }
    $pool = 'frameworkoncore-webcontrolsprobe'
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
    & $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:localhost" -f $Port) | Out-Null
    & $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
    & $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
    & icacls $site /grant "IIS AppPool\${pool}:(OI)(CI)(M)" /T /Q | Out-Null
    $web = (Get-Item (Join-Path $env:SystemRoot 'Microsoft.NET\Framework64\v4.0.30319\System.Web.dll')).VersionInfo.FileVersion
    $iis = (Get-Item (Join-Path $env:SystemRoot 'System32\inetsrv\w3wp.exe')).VersionInfo.ProductVersion
    $os = [Environment]::OSVersion.Version
    try {
        foreach ($name in $Control) {
            Use-Component $name
            Wait-Site "http://localhost:$Port"
            Invoke-SiteLayers "http://localhost:$Port" $golden -Recording
            "$(Get-Date -Format 'yyyy-MM-dd'), .NET Framework 4.8 (System.Web $web), IIS $iis, Windows $os" | Set-Content (Join-Path $golden 'recorded.txt') -Encoding utf8
            Invoke-Checked 'the test cases report' { dotnet $program report --control $Current --golden $golden --out $reports }
        }
    }
    finally {
        & $appcmd delete site $pool 2>&1 | Out-Null
        & $appcmd delete apppool $pool 2>&1 | Out-Null
    }
    exit 0
}

if (-not ($Windows -or $Linux)) { throw '-Record, -Windows or -Linux' }

# The probe site converted with the converter's default choices, built, with a Dockerfile (for -Linux).
$converted = Join-Path $work 'converted'
Remove-Folder $converted
Invoke-Checked 'the converter build' { dotnet build (Join-Path $repo 'src\FrameworkOnCore.Converter') --nologo -v q | Out-Null }
$converter = Join-Path $repo 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
Invoke-Checked 'the conversion' { dotnet $converter (Join-Path $sample 'WebControlsProbe.csproj') --out $converted --root $sample --deploy container *> (Join-Path $work 'convert.log') }

$exit = 0
if ($Windows) {
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList 'bin\WebControlsProbe.dll' -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'windows-out.log') -RedirectStandardError (Join-Path $work 'windows-err.log')
    try {
        foreach ($name in $Control) {
            Use-Component $name
            # Each component's own (its report can be made again from it: report --port <this folder>).
            $out = Join-Path $work "windows\$Current"
            Remove-Folder $out
            New-Item -ItemType Directory $out -Force | Out-Null
            Invoke-Checked 'the API layer' { dotnet $program api --control $Current --out (Join-Path $out 'api') }
            Wait-Site "http://localhost:$Port"
            Invoke-SiteLayers "http://localhost:$Port" $out
            dotnet $program report --control $Current --golden $golden --port $out --out $reports --platform windows
            if ($LASTEXITCODE -ne 0) { $exit = 1; "$Current : FAIL" } else { "$Current : OK" }
        }
    }
    # The tree: the application runs in a worker process its process starts (FrameworkOnCore's System.Web).
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
}

if ($Linux) {
    $ErrorActionPreference = 'Continue'   # docker's stderr is not an error; exit codes are checked
    # The API layer in .NET's ASP.NET Core runtime container (the converted site's base): the program published for any platform.
    $published = Join-Path $work 'linux-program'
    Remove-Folder $published
    Invoke-Checked 'the parity program publish' { dotnet publish $here -f net10.0 -o $published -nologo -v q | Out-Null }
    $image = 'w2l-webcontrolsprobe'
    docker build -t $image $converted *> (Join-Path $work 'docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed (see $work\docker-build.log)" }
    cmd /c "docker rm -f $image" 2>&1 | Out-Null
    # The golden data was recorded from IIS's own machine (a local client: IIS's detailed error pages, the application's
    # error answers left): this machine taken as the container's (HttpErrors:LocalAddresses).
    docker run -d --name $image -p "${Port}:8080" -e 'HttpErrors__LocalAddresses=*' $image | Out-Null
    try {
        foreach ($name in $Control) {
            Use-Component $name
            # Each component's own (its report can be made again from it: report --port <this folder>).
            $out = Join-Path $work "linux\$Current"
            Remove-Folder $out
            New-Item -ItemType Directory (Join-Path $out 'api') -Force | Out-Null
            # One container at a time: the API layer's runs and ends before the next (the site's keeps running).
            Invoke-Checked 'the API layer (Linux)' { docker run --rm -v "${published}:/p:ro" -v "$(Join-Path $out 'api'):/o" mcr.microsoft.com/dotnet/aspnet:10.0 dotnet /p/WebControlsParity.dll api --control $Current --out /o }
            Wait-Site "http://localhost:$Port"
            Invoke-SiteLayers "http://localhost:$Port" $out
            dotnet $program report --control $Current --golden $golden --port $out --out $reports --platform linux
            if ($LASTEXITCODE -ne 0) { $exit = 1; "$Current : FAIL" } else { "$Current : OK" }
        }
    }
    finally {
        docker logs $image *> (Join-Path $work 'linux-server.log')
        cmd /c "docker rm -f $image" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
exit $exit
