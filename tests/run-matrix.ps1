<#
The environment matrix: the test projects run under other cultures and time zones (and on Linux, its case-sensitive
file system), to find what assumes the machine's. Nothing in the tests is edited: tests/Shared/TestEnvironment.cs (in
the three projects) applies FOC_TEST_CULTURE ("ja-JP", "tr-TR"...; "invariant": the invariant culture) to the culture
and UI culture (SqlClientTests keeps English messages: its UI culture stays en-US) and FOC_TEST_TZ to TimeZoneInfo.Local.

  .\tests\run-matrix.ps1                       # Windows: the three projects under the six cultures
  .\tests\run-matrix.ps1 -Os Linux             # Linux (Docker): RuntimeUnitTests, FrameworkOnCore.Tests and
                                               # SqlClientTests under en-US, tr-TR, de-DE x UTC, Asia/Tokyo, America/Los_Angeles
  .\tests\run-matrix.ps1 -Os Both
  .\tests\run-matrix.ps1 -Projects RuntimeUnitTests -Cultures tr-TR -TimeZones 'Pacific Standard Time' -Filter 'FullyQualifiedName~Iis'

-Cultures, -TimeZones: Windows' runs (each culture under each zone; '' = the machine's zone). On Windows the zone is
  set inside the test process by reflection (TestEnvironment.cs: TimeZoneInfo's cached local zone, checked: the run
  fails at once if it was not taken); native code and other processes keep the machine's.
-LinuxCultures, -LinuxTimeZones: Linux's runs (TZ: the whole process's zone), one container per project, one after the
  other; FrameworkOnCore.Tests' container runs SqlClientTests too (unless -Filter).
-Filter: dotnet test --filter. -NoBuild: Windows' builds as they are.
-Out: where the results go (default tests\TestResults\matrix): <os>-<project>-<culture>-<zone>.trx per run,
  summary.md (the table) and failures.txt (each failed test by run).
After Linux runs Docker Desktop is stopped and WSL shut down (the machine's memory), unless -KeepDocker.
#>
param(
    [ValidateSet('Windows', 'Linux', 'Both')][string]$Os = 'Windows',
    [string[]]$Projects = @('RuntimeUnitTests', 'FrameworkOnCore.Tests', 'SqlClientTests'),
    [string[]]$Cultures = @('ja-JP', 'en-US', 'invariant', 'tr-TR', 'de-DE', 'ar-SA'),
    [string[]]$TimeZones = @(''),
    [string[]]$LinuxCultures = @('en-US', 'tr-TR', 'de-DE'),
    [string[]]$LinuxTimeZones = @('UTC', 'Asia/Tokyo', 'America/Los_Angeles'),
    [string]$Filter,
    [string]$Out,
    [switch]$NoBuild,
    [switch]$KeepDocker)
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if (-not $Out) { $Out = Join-Path $repo 'tests\TestResults\matrix' }
New-Item -ItemType Directory -Force $Out | Out-Null
$Out = (Resolve-Path $Out).Path

# dotnet test's summary and failures, in English or Japanese (the script ASCII: Windows PowerShell reads it as the machine's code page).
$summaryLines = '^\s+(Failed|' + (-join [char[]](0x5931, 0x6557)) + ') |' + (-join [char[]](0x5408, 0x8A08)) + ':|Total tests|Passed!|Failed!'
function Safe([string]$s) { if ($s) { $s -replace '[\\/: ]', '_' } else { '' } }

if ($Os -ne 'Linux') {
    foreach ($project in $Projects) {
        $csproj = Join-Path $repo "tests\$project\$project.csproj"
        if (-not $NoBuild) {
            Write-Host "== build $project"
            dotnet build $csproj -nologo -v q -clp:ErrorsOnly | Out-Host
            if ($LASTEXITCODE -ne 0) { throw "$project does not build" }
        }
        foreach ($culture in $Cultures) {
            foreach ($zone in $TimeZones) {
                $name = "windows-$project-$culture" + $(if ($zone) { '-' + (Safe $zone) } else { '' })
                Remove-Item (Join-Path $Out "$name.trx") -ErrorAction SilentlyContinue
                Write-Host "== $project culture=$culture zone=$(if ($zone) { $zone } else { '(machine)' })"
                $env:FOC_TEST_CULTURE = $culture
                if ($zone) { $env:FOC_TEST_TZ = $zone } else { Remove-Item Env:FOC_TEST_TZ -ErrorAction SilentlyContinue }
                $arguments = @('test', $csproj, '--no-build', '-nologo', '--logger', "trx;LogFileName=$(Join-Path $Out "$name.trx")")
                if ($Filter) { $arguments += @('--filter', $Filter) }
                try { & dotnet @arguments 2>$null | Select-String -Pattern $summaryLines | Out-Host }
                finally { Remove-Item Env:FOC_TEST_CULTURE, Env:FOC_TEST_TZ -ErrorAction SilentlyContinue }
            }
        }
    }
}

if ($Os -ne 'Windows') {
    docker info *> $null
    if ($LASTEXITCODE -ne 0) {
        Write-Host '== starting Docker Desktop'
        Start-Process "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe"
        foreach ($i in 1..90) { Start-Sleep -Seconds 2; docker info *> $null; if ($LASTEXITCODE -eq 0) { break } }
        if ($LASTEXITCODE -ne 0) { throw 'Docker did not start' }
    }
    $matrix = @(foreach ($c in $LinuxCultures) { foreach ($z in $LinuxTimeZones) { "$c@$z" } })
    try {
        # One container at a time (the machine's memory).
        if ($Projects -contains 'RuntimeUnitTests') {
            Get-ChildItem $Out -Filter 'linux-RuntimeUnitTests-*.trx' | Remove-Item
            Write-Host "== Linux RuntimeUnitTests: $($matrix -join ' ')"
            $env:MATRIX = $matrix -join ' '
            $env:FILTER = $Filter
            try {
                docker run --rm -m 6g -e MATRIX -e FILTER -v "${repo}:/repo:ro" -v "${Out}:/results" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh | Out-Host
            }
            finally { Remove-Item Env:MATRIX, Env:FILTER -ErrorAction SilentlyContinue }
        }
        if ($Projects -contains 'FrameworkOnCore.Tests' -or $Projects -contains 'SqlClientTests') {
            Get-ChildItem $Out -Filter 'linux-*.trx' | Where-Object { $_.Name -notlike 'linux-RuntimeUnitTests-*' } | Remove-Item
            Write-Host "== Linux FrameworkOnCore.Tests, SqlClientTests: $($matrix -join ' ')"
            $linux = @{ Matrix = $matrix; Results = $Out }
            if ($Filter) { $linux.Filter = $Filter }
            & (Join-Path $repo 'tests\FrameworkOnCore.Tests\run-tests-linux.ps1') @linux | Out-Host
        }
    }
    finally {
        if (-not $KeepDocker) {
            Write-Host '== stopping Docker Desktop, shutting WSL down'
            Get-Process 'Docker Desktop', 'com.docker.backend' -ErrorAction SilentlyContinue | Stop-Process -Force
            wsl --shutdown
        }
    }
}

# The table and the failures, from every run's results in -Out.
$rows = @(); $failures = @()
foreach ($trx in Get-ChildItem $Out -Filter '*.trx' | Sort-Object Name) {
    [xml]$x = Get-Content -LiteralPath $trx.FullName -Raw -Encoding UTF8
    $c = $x.TestRun.ResultSummary.Counters
    $rows += [pscustomobject]@{ Run = $trx.BaseName; Total = [int]$c.total; Passed = [int]$c.passed; Failed = [int]$c.failed; Skipped = [int]$c.total - [int]$c.executed }
    foreach ($r in $x.TestRun.Results.UnitTestResult | Where-Object { $_.outcome -eq 'Failed' }) {
        $message = "$($r.Output.ErrorInfo.Message)" -split "`n" | Select-Object -First 1
        $failures += "$($trx.BaseName)`t$($r.testName)`t$message"
    }
}
$table = @('| Run | Total | Passed | Failed | Skipped |', '|---|---:|---:|---:|---:|') +
    @($rows | ForEach-Object { "| $($_.Run) | $($_.Total) | $($_.Passed) | $($_.Failed) | $($_.Skipped) |" })
Set-Content -LiteralPath (Join-Path $Out 'summary.md') -Value $table -Encoding UTF8
Set-Content -LiteralPath (Join-Path $Out 'failures.txt') -Value $failures -Encoding UTF8
$table | Out-Host
Write-Host "$($failures.Count) failure(s): $(Join-Path $Out 'failures.txt')"
