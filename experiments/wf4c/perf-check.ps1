# Throughput and latency of a sample's page, the same load (bombardier: -Connections at once for -Seconds, after a warm-up)
# on IIS (.NET Framework 4.8), on the converted application on Windows (Kestrel) and on it in its Linux container. On
# Linux the load runs in the application's container's network (docker run --network container:...): inside Docker's Linux
# machine, without Windows' port forwarding (Docker Desktop's), which load-check.ps1's numbers went through. -Trace: a CPU
# trace of the Linux run's worker during the load (dotnet-trace in an SDK container sharing the application's processes
# and /tmp), as speedscope JSON, and its busiest frames (perf-frames.py). Numbers to compare on one machine, one run at a
# time (CLAUDE.md); the load generator shares the machine's CPUs with the server in every run alike.
# -Diagnose: the Linux run's GC while the load runs (dotnet-counters: the collections, the allocation rate, the time in
# GC) and after it the worker's heap (dotnet-dump: the objects waiting for their finalizers, the heap's types), into the
# perf folder (counters.csv, finalizequeue.txt, heap.txt).
#   .\experiments\wf4c\perf-check.ps1 -Name SessionProbe -Path /Default.aspx [-Only iis,windows,linux] [-Trace] [-Diagnose]
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [string]$Path = '/Default.aspx',
    [int]$Connections = 32,
    [int]$Seconds = 20,
    [string[]]$Only = @('iis', 'windows', 'linux'),
    [switch]$Trace,
    [switch]$Diagnose,
    [int]$Port = 5096,
    [string]$Tools = (Join-Path $env:TEMP 'perf')
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$out = Join-Path $work 'perf'
New-Item -ItemType Directory $out -Force | Out-Null
$bombardier = Join-Path $Tools 'bombardier-windows-amd64.exe'
if (-not (Test-Path $bombardier)) { throw "$bombardier (and bombardier-linux-amd64): github.com/codesenberg/bombardier's releases" }

function Wait-Answer([string]$url) {
    foreach ($attempt in 1..600) {
        if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 $url) -eq '200') { return }
        Start-Sleep -Milliseconds 200
    }
    throw "$url did not answer 200"
}

# bombardier's JSON result: requests per second, latency percentiles (µs), errors and statuses.
function Summary([string]$json) {
    $r = ($json | ConvertFrom-Json).result
    $p = $r.latency.percentiles
    $codes = 'other ' + ($r.req1xx + $r.req3xx + $r.req4xx + $r.req5xx + $r.others)
    '{0:N0} req/s, latency ms p50 {1:N2} p90 {2:N2} p99 {3:N2}, 2xx {4}, {5}' -f $r.rps.mean, ($p.'50' / 1000), ($p.'90' / 1000), ($p.'99' / 1000), $r.req2xx, $codes
}

function Load-Windows([string]$url) {
    & $bombardier -c $Connections -d 5s -o json -p r $url | Out-Null     # warm-up
    Summary ((& $bombardier -c $Connections -d "${Seconds}s" -l -o json -p r $url) -join '')
}

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
        Wait-Answer "http://localhost:$Port$Path"
        "IIS:     " + (Load-Windows "http://localhost:$Port$Path")
    }
    finally {
        & $appcmd delete site $pool 2>&1 | Out-Null
        & $appcmd delete apppool $pool 2>&1 | Out-Null
    }
}

$converted = Join-Path $work 'converted'
if ($Only -contains 'windows') {
    if (-not (Test-Path $converted)) { throw "${converted}: sample-parity.ps1 -Name $Name -Windows first" }
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $out 'windows-out.log') -RedirectStandardError (Join-Path $out 'windows-err.log')
    try {
        Wait-Answer "http://localhost:$Port$Path"
        "Windows: " + (Load-Windows "http://localhost:$Port$Path")
    }
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
}

if ($Only -contains 'linux') {
    $ErrorActionPreference = 'Continue'
    $image = "w2l-perf-$($Name.ToLowerInvariant())"
    $app = "$image-app"
    $volume = "$image-tmp"
    docker build -t $image $converted *> (Join-Path $out 'docker-build.log')
    if ($LASTEXITCODE -ne 0) { throw "docker build failed" }
    cmd /c "docker rm -f $app" 2>&1 | Out-Null
    cmd /c "docker volume rm $volume" 2>&1 | Out-Null
    # /tmp shared with the tracer (the runtime's diagnostics sockets are there).
    docker run -d --name $app -v "${volume}:/tmp" $image | Out-Null
    $toolsMount = "$($Tools.Replace('\', '/')):/perf:ro"
    $load = { param($seconds, $latency)
        $arguments = @('run', '--rm', '--network', "container:$app", '-v', $toolsMount, 'mcr.microsoft.com/dotnet/aspnet:10.0',
            '/perf/bombardier-linux-amd64', '-c', $Connections, '-d', "${seconds}s", '-o', 'json', '-p', 'r')
        if ($latency) { $arguments += '-l' }
        $arguments += "http://localhost:8080$Path"
        (docker @arguments) -join ''
    }
    try {
        foreach ($attempt in 1..300) {
            $status = docker run --rm --network "container:$app" mcr.microsoft.com/dotnet/aspnet:10.0 bash -c "exec 3<>/dev/tcp/127.0.0.1/8080 && printf 'GET $Path HTTP/1.0\r\nHost: localhost\r\n\r\n' >&3 && head -c 12 <&3" 2>$null
            if ($status -match ' 200') { break }
            Start-Sleep -Seconds 1
        }
        & $load 5 $false | Out-Null    # warm-up
        $tracer = "$image-tracer"
        $worker = "`$(/root/.dotnet/tools/dotnet-trace ps | awk '{print `$1}' | sort -n | tail -1)"
        if ($Diagnose) {
            cmd /c "docker rm -f $tracer" 2>&1 | Out-Null
            docker run -d --name $tracer --pid "container:$app" -v "${volume}:/tmp" -v "$($out.Replace('\', '/')):/out" mcr.microsoft.com/dotnet/sdk:10.0 sleep infinity | Out-Null
            docker exec $tracer bash -c "dotnet tool install -g dotnet-trace; dotnet tool install -g dotnet-counters; dotnet tool install -g dotnet-dump" | Out-Null
            Remove-Item (Join-Path $out 'counters.csv') -ErrorAction SilentlyContinue
            docker exec -d $tracer bash -c "/root/.dotnet/tools/dotnet-counters collect -p $worker --refresh-interval 1 --duration 00:00:00:$($Seconds - 4) --format csv -o /out/counters.csv --counters System.Runtime > /out/counters.log 2>&1"
            Start-Sleep -Seconds 2
        }
        if ($Trace) {
            # An SDK container with dotnet-trace, sharing the application's processes and /tmp; the trace of the worker (the
            # application's process its supervisor started: the newest dotnet process) runs while the load does.
            cmd /c "docker rm -f $tracer" 2>&1 | Out-Null
            docker run -d --name $tracer --pid "container:$app" -v "${volume}:/tmp" -v "$($out.Replace('\', '/')):/out" mcr.microsoft.com/dotnet/sdk:10.0 sleep infinity | Out-Null
            docker exec $tracer bash -c "dotnet tool install -g dotnet-trace" | Out-Null
            Remove-Item (Join-Path $out 'linux-trace*') -ErrorAction SilentlyContinue
            $collect = "pid=`$(/root/.dotnet/tools/dotnet-trace ps | awk '{print `$1}' | sort -n | tail -1); /root/.dotnet/tools/dotnet-trace collect -p `$pid --duration 00:00:00:$($Seconds - 4) --format speedscope -o /out/linux-trace.nettrace > /out/trace.log 2>&1"
            docker exec -d $tracer bash -c $collect
            Start-Sleep -Seconds 2
        }
        "Linux:   " + (Summary (& $load $Seconds $true))
        if ($Diagnose) {
            Start-Sleep -Seconds 3
            docker exec $tracer bash -c "rm -f /tmp/heap.dmp; /root/.dotnet/tools/dotnet-dump collect -p $worker --type Heap -o /tmp/heap.dmp > /dev/null && /root/.dotnet/tools/dotnet-dump analyze /tmp/heap.dmp -c 'finalizequeue -stat' -c exit > /out/finalizequeue.txt 2>&1; /root/.dotnet/tools/dotnet-dump analyze /tmp/heap.dmp -c 'dumpheap -stat' -c exit > /out/heap.txt 2>&1; rm -f /tmp/heap.dmp"
            python (Join-Path $PSScriptRoot 'perf-counters.py') (Join-Path $out 'counters.csv')
        }
        if ($Trace) {
            foreach ($attempt in 1..60) { if (Get-ChildItem $out -Filter 'linux-trace*.speedscope.json' -ErrorAction SilentlyContinue) { break }; Start-Sleep -Seconds 2 }
            # Written in pieces: read once its length stays.
            $length = -1
            foreach ($attempt in 1..60) {
                $file = Get-ChildItem $out -Filter 'linux-trace*.speedscope.json' | Select-Object -Last 1
                if ($file.Length -eq $length) { break }
                $length = $file.Length
                Start-Sleep -Seconds 3
            }
            $speedscope = Get-ChildItem $out -Filter '*.speedscope.json' | Sort-Object LastWriteTime | Select-Object -Last 1
            if ($speedscope) { python (Join-Path $PSScriptRoot 'perf-frames.py') $speedscope.FullName } else { Get-Content (Join-Path $out 'trace.log') -Tail 10 }
        }
    }
    finally {
        docker logs $app *> (Join-Path $out 'linux-app.log')
        cmd /c "docker rm -f $image-tracer" 2>&1 | Out-Null
        cmd /c "docker rm -f $app" 2>&1 | Out-Null
        cmd /c "docker volume rm $volume" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
