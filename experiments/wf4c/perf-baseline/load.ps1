# The environment's own cost, without System.Web: perf-baseline (Program.cs) under perf-check.ps1's load (bombardier,
# -Connections at once for -Seconds after a 5 second warm-up) on Windows and in its Linux container (the load in the
# container's network), each path in turn. Docker Desktop running (CLAUDE.md: one container at a time, closed after).
#   .\experiments\wf4c\perf-baseline\load.ps1 [-Only windows,linux] [-Paths /plain,/thread,/crypto,/culture]
param(
    [string[]]$Only = @('windows', 'linux'),
    [string[]]$Paths = @('/plain', '/thread', '/crypto', '/culture'),
    [int]$Connections = 32,
    [int]$Seconds = 15,
    [int]$Port = 5098,
    [string]$Tools = (Join-Path $env:TEMP 'perf')
)

$ErrorActionPreference = 'Stop'
$out = Join-Path $PSScriptRoot 'out'
dotnet publish $PSScriptRoot -c Release -o $out -v q -nologo | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed' }

function Summary([string]$json) {
    $r = ($json | ConvertFrom-Json).result
    '{0:N0} req/s, p99 {1:N2} ms' -f $r.rps.mean, ($r.latency.percentiles.'99' / 1000)
}

if ($Only -contains 'windows') {
    $bombardier = Join-Path $Tools 'bombardier-windows-amd64.exe'
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList 'baseline.dll' -WorkingDirectory $out -PassThru -WindowStyle Hidden
    Start-Sleep 3
    try {
        foreach ($path in $Paths) {
            & $bombardier -c $Connections -d 5s -o json -p r "http://localhost:$Port$path" | Out-Null
            "Windows ${path}: " + (Summary ((& $bombardier -c $Connections -d "${Seconds}s" -l -o json -p r "http://localhost:$Port$path") -join ''))
        }
    }
    finally { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
}

if ($Only -contains 'linux') {
    $ErrorActionPreference = 'Continue'
    $image = 'w2l-perf-baseline'
    $app = "$image-app"
    $toolsMount = ((($Tools -split '\\') -join '/') + ':/perf:ro')
    docker build -t $image $PSScriptRoot *> $null
    if ($LASTEXITCODE -ne 0) { throw 'docker build failed' }
    cmd /c "docker rm -f $app" 2>&1 | Out-Null
    docker run -d --name $app $image | Out-Null
    Start-Sleep 4
    try {
        foreach ($path in $Paths) {
            $arguments = @('run', '--rm', '--network', "container:$app", '-v', $toolsMount, 'mcr.microsoft.com/dotnet/aspnet:10.0',
                '/perf/bombardier-linux-amd64', '-c', $Connections, '-o', 'json', '-p', 'r')
            docker @arguments -d 5s "http://localhost:8080$path" | Out-Null
            "Linux   ${path}: " + (Summary ((docker @arguments -d "${Seconds}s" -l "http://localhost:8080$path") -join ''))
        }
    }
    finally {
        cmd /c "docker rm -f $app" 2>&1 | Out-Null
        cmd /c "docker image rm $image" 2>&1 | Out-Null
    }
}
