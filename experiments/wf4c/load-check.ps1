# A sample's page under load, on IIS (.NET Framework 4.8), the converted application on Windows and in its Linux container:
# the time to the first answer after the start (compilation included), then rounds of requests at once (curl --parallel):
# the answers that were not 200, the time each round took, the memory after it (IIS: the pool's w3wp; Windows: the
# application's process tree; Linux: the container's, docker stats). Numbers to look at, not compared: the machines differ
# from run to run. Run sample-parity.ps1 -Record and -Windows for the sample first (its IIS site and its conversion).
#   .\experiments\wf4c\load-check.ps1 -Name SessionProbe -Path /Default.aspx [-Requests 2000] [-Parallel 20] [-Rounds 3] [-Only iis,windows,linux]
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [string]$Path = '/Default.aspx',
    [int]$Requests = 2000,
    [int]$Parallel = 20,
    [int]$Rounds = 3,
    [string[]]$Only = @('iis', 'windows', 'linux'),
    [int]$Port = 5096
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$url = "http://localhost:$Port"

function First-Answer([datetime]$started) {
    foreach ($attempt in 1..600) {
        $status = curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$url$Path"
        if ($status -eq '200') { return [int]((Get-Date) - $started).TotalMilliseconds }
        Start-Sleep -Milliseconds 200
    }
    throw "$url$Path did not answer 200"
}

# A round: the requests at once, Parallel at a time; the answers' statuses counted.
function Round([scriptblock]$memory) {
    $started = Get-Date
    $codes = curl.exe -s -o NUL --parallel --parallel-immediate --parallel-max $Parallel -w "%{http_code}\n" "$url$Path`?n=[1-$Requests]"
    $elapsed = ((Get-Date) - $started).TotalSeconds
    $bad = @($codes | Where-Object { $_ -and $_ -ne '200' }).Count
    '{0} requests in {1:N1} s ({2:N0}/s), not 200: {3}, memory {4}' -f $Requests, $elapsed, ($Requests / $elapsed), $bad, (& $memory)
}

function Megabytes([long]$bytes) { '{0:N0} MB' -f ($bytes / 1MB) }

if ($Only -contains 'iis') {
    $appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
    $pool = "frameworkoncore-$($Name.ToLowerInvariant())"
    $site = Join-Path $work 'iis'
    if (-not (Test-Path $site)) { throw "${site}: sample-parity.ps1 -Name $Name -Record first" }
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
    & $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
    & $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:localhost" -f $Port) | Out-Null
    & $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
    & $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
    try {
        "=== IIS"
        "first answer after the start: $(First-Answer (Get-Date)) ms"
        $memory = {
            $w3wp = Get-CimInstance Win32_Process -Filter "Name='w3wp.exe'" | Where-Object { $_.CommandLine -match [regex]::Escape($pool) }
            Megabytes (($w3wp | ForEach-Object { (Get-Process -Id $_.ProcessId).WorkingSet64 } | Measure-Object -Sum).Sum)
        }
        foreach ($i in 1..$Rounds) { Round $memory }
    }
    finally {
        & $appcmd delete site $pool 2>&1 | Out-Null
        & $appcmd delete apppool $pool 2>&1 | Out-Null
    }
}

$converted = Join-Path $work 'converted'
if ($Only -contains 'windows') {
    if (-not (Test-Path $converted)) { throw "${converted}: sample-parity.ps1 -Name $Name -Windows first" }
    "=== Windows (converted)"
    $env:ASPNETCORE_URLS = $url
    $started = Get-Date
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'load-out.log') -RedirectStandardError (Join-Path $work 'load-err.log')
    try {
        "first answer after the start: $(First-Answer $started) ms"
        $memory = {
            # The supervisor and its worker (the application runs in a process its process starts).
            $ids = @($process.Id) + @(Get-CimInstance Win32_Process -Filter "ParentProcessId=$($process.Id)" | ForEach-Object { $_.ProcessId })
            Megabytes (($ids | ForEach-Object { (Get-Process -Id $_ -ErrorAction SilentlyContinue).WorkingSet64 } | Measure-Object -Sum).Sum)
        }
        foreach ($i in 1..$Rounds) { Round $memory }
    }
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
}

if ($Only -contains 'linux') {
    $ErrorActionPreference = 'Continue'
    $image = "w2l-load-$($Name.ToLowerInvariant())"
    docker build -t $image $converted *> (Join-Path $work 'load-docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed" }
    cmd /c "docker rm -f $image" 2>&1 | Out-Null
    "=== Linux (converted, container)"
    $started = Get-Date
    docker run -d --name $image -p "${Port}:8080" $image | Out-Null
    try {
        "first answer after the start: $(First-Answer $started) ms"
        $memory = { (docker stats --no-stream --format '{{.MemUsage}}' $image) -replace ' /.*', '' }
        foreach ($i in 1..$Rounds) { Round $memory }
    }
    finally {
        cmd /c "docker rm -f $image" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
