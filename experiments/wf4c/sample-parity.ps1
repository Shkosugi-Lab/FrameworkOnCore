# A sample (samples\<Name>) as its users see it, against the original on IIS: -Record takes the golden data from the
# sample built for .NET Framework 4.8 and run on IIS (FrameworkOnCore.ParityTest record, the sample's
# parity-scenario.json, into samples\<Name>\golden-webforms.json); -Windows and -Linux convert the sample with the
# converter (its default choices), run it (on Windows from its bin, on Linux from the Dockerfile the converter writes)
# and compare (ParityTest verify). As corpora\record-webforms-golden.ps1 and run-linux.ps1 do for the corpora, for a
# sample whose packages run-sample.ps1's template does not have (System.Web.Mobile's, the charts').
#
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Record      # IIS (an administrator's shell)
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Windows
#   .\experiments\wf4c\sample-parity.ps1 -Name MobileProbe -Linux       # Docker
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [switch]$Record,
    [switch]$Windows,
    [switch]$Linux,
    [int]$Port = 5096
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$sample = Join-Path $repo "samples\$Name"
$scenario = Join-Path $sample 'parity-scenario.json'
$golden = Join-Path $sample 'golden-webforms.json'
$work = Join-Path $PSScriptRoot "_samples\$Name"
New-Item -ItemType Directory $work -Force | Out-Null
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'
if (-not (Test-Path $verifier)) {
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
    & 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe' (Join-Path $site "$Name.csproj") /nologo /v:q /p:Configuration=Debug
    if ($LASTEXITCODE -ne 0) { throw 'the sample did not build for .NET Framework' }
    $pool = "frameworkoncore-$($Name.ToLowerInvariant())"
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
    & $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:localhost" -f $Port) | Out-Null
    & $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
    & $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
    # The pool's identity reads the site (IIS answers 500.19 otherwise) and writes App_Data.
    & icacls $site /grant "IIS AppPool\${pool}:(OI)(CI)(M)" /T /Q | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier record --url "http://localhost:$Port" --scenario $scenario --out $golden
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
& dotnet $converter (Join-Path $sample "$Name.csproj") --out $converted --root $sample --deploy container
if ($LASTEXITCODE -ne 0) { throw 'the conversion failed' }

$exit = 0
if ($Windows) {
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'windows-out.log') -RedirectStandardError (Join-Path $work 'windows-err.log')
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier verify --url "http://localhost:$Port" --scenario $scenario --golden $golden
        $exit = $LASTEXITCODE
    }
    # The tree: the application runs in a worker process its process starts (FrameworkOnCore's System.Web).
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
}

if ($Linux) {
    $ErrorActionPreference = 'Continue'   # docker's stderr is not an error; exit codes are checked
    $image = "w2l-$($Name.ToLowerInvariant())"
    docker build -t $image $converted *> (Join-Path $work 'docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed (see $work\docker-build.log)" }
    cmd /c "docker rm -f $image" 2>&1 | Out-Null
    docker run -d --name $image -p "${Port}:8080" $image | Out-Null
    try {
        Wait-Site "http://localhost:$Port"
        & dotnet $verifier verify --url "http://localhost:$Port" --scenario $scenario --golden $golden
        $exit = $LASTEXITCODE
    }
    finally {
        docker logs $image *> (Join-Path $work 'linux-server.log')
        cmd /c "docker rm -f $image" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
exit $exit
