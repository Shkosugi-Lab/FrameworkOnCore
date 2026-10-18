# Every line that differs between a sample's golden data and what its converted application shows: the application
# converted by sample-parity.ps1 (-Windows or -Linux leaves it in _samples\<Name>\converted) run here, natively, its
# answers recorded (ParityTest record) and compared snapshot by snapshot with the golden data, line by line (the
# verifier stops at the first five). For the probes whose pages are lists of "case = value" lines.
#
#   .\experiments\wf4c\sample-diff.ps1 -Name BclProbe              # after sample-parity.ps1 -Name BclProbe -Windows
#   .\experiments\wf4c\sample-diff.ps1 -Name BclProbe -Recorded x  # an earlier recording (a container's), no run
param(
    [Parameter(Mandatory = $true)][string]$Name,
    [string]$Recorded,
    [int]$Port = 5098
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$sample = Join-Path $repo "samples\$Name"
$work = Join-Path $PSScriptRoot "_samples\$Name"
$verifier = Join-Path $repo 'tools\FrameworkOnCore.ParityTest\bin\alt\FrameworkOnCore.ParityTest.dll'

if (-not $Recorded) {
    $Recorded = Join-Path $work 'recorded-port.json'
    $converted = Join-Path $work 'converted'
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput (Join-Path $work 'diff-out.log') -RedirectStandardError (Join-Path $work 'diff-err.log')
    try {
        foreach ($attempt in 1..90) {
            try { Invoke-WebRequest "http://localhost:$Port/" -UseBasicParsing -TimeoutSec 60 | Out-Null; break }
            catch { if ($_.Exception.Response) { break }; Start-Sleep -Seconds 2 }
        }
        & dotnet $verifier record --url "http://localhost:$Port" --scenario (Join-Path $sample 'parity-scenario.json') --out $Recorded | Out-Null
        if ($LASTEXITCODE -ne 0) { throw 'recording the converted application failed' }
    }
    finally {
        if (-not $process.HasExited) { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }
    }
}

$golden = (Get-Content (Join-Path $sample 'golden-webforms.json') -Raw -Encoding UTF8 | ConvertFrom-Json).Snapshots
$actual = (Get-Content $Recorded -Raw -Encoding UTF8 | ConvertFrom-Json).Snapshots
$total = 0
foreach ($expected in $golden) {
    $got = $actual | Where-Object { $_.Name -eq $expected.Name } | Select-Object -First 1
    if (-not $got) { Write-Host "## $($expected.Name): not recorded" -ForegroundColor Red; continue }
    $count = [Math]::Max($expected.Text.Count, $got.Text.Count)
    $differences = @()
    for ($i = 0; $i -lt $count; $i++) {
        $e = if ($i -lt $expected.Text.Count) { $expected.Text[$i] } else { '(none)' }
        $a = if ($i -lt $got.Text.Count) { $got.Text[$i] } else { '(none)' }
        if ($e -ne $a) { $differences += "  - $e`n  + $a" }
    }
    if ($differences.Count -eq 0) { continue }
    $total += $differences.Count
    Write-Output "## $($expected.Name): $($differences.Count) of $count lines"
    $differences | Write-Output
}
Write-Output "TOTAL: $total lines differ"
