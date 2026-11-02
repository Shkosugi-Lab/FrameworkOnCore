<#
One entry to the test sets of tests/README.md: it only calls the existing commands, in order, and stops at the first
that fails. What each command checks, and the commands themselves to run one by one: tests/README.md.

  .\tests\run-tests.ps1 -Set quick      # the three xunit projects (no stress), the translations' check: ~5 min, nothing needed
  .\tests\run-tests.ps1 -Set commit     # after a converter change: FrameworkOnCore.Tests, be/wt converted against their
                                        # baselines, be/wt compared with IIS's golden data on Windows (~5 min)
  .\tests\run-tests.ps1 -Set runtime    # after a FrameworkOnCore.Runtime change: the packages packed (a published version
                                        # stops it: raise frameworkOnCoreVersion), the three xunit projects, then commit's be/wt
  .\tests\run-tests.ps1 -Set full       # before a release: runtime, the culture matrix, wt's and be's every operation, and Linux
                                        # (be's every operation there too)
  -Linux: Linux's runs too (commit, runtime; full has them): the tests' containers, then be and wt, one container at a time.

The rules of this machine (CLAUDE.md): the corpora are be and wt only; containers one after the other, never in the
background; afterwards Docker Desktop is stopped and WSL shut down, and SQL Server Express stopped if this started it.
Corpora: corpora\fetch.ps1 once. Edge: ParityTest drives it (headless) for be/wt.
#>
param(
    [Parameter(Mandatory = $true)][ValidateSet('quick', 'commit', 'runtime', 'full')][string]$Set,
    [switch]$Linux)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
Set-Location $repo
$wf4c = Join-Path $repo 'experiments\wf4c'
$started = Get-Date
$log = @()
$failedSteps = @()

# -Check exit: the command's exit code (dotnet test, verify-windows.ps1, check.ps1, the Linux scripts: run-linux.ps1,
# run-tests-linux.*); throw: only what it throws (scripts that report by exceptions: pack, convert-corpora).
# -Continue: a failure is reported and the next steps still run (Linux's runs, each one's result); the set fails at the end.
function Step([string]$name, [scriptblock]$command, [ValidateSet('exit', 'throw')][string]$Check = 'exit', [switch]$Continue) {
    Write-Host "`n===== $name" -ForegroundColor Cyan
    $t = Get-Date
    $global:LASTEXITCODE = 0
    $failed = $null
    try {
        & $command | Out-Host
        if ($Check -eq 'exit' -and $LASTEXITCODE -ne 0) { $failed = "exit $LASTEXITCODE" }
    }
    catch { $failed = "$_" }
    $script:log += '{0,-48} {1,6:n0} s  {2}' -f $name, ((Get-Date) - $t).TotalSeconds, $(if ($failed) { "FAILED: $failed" } else { 'ok' })
    if ($failed) { $script:failedSteps += $name }
    if ($failed -and -not $Continue) { throw "$name failed: $failed" }
}

function Test-Project([string]$project, [string]$filter) {
    $arguments = @('test', "tests\$project\$project.csproj", '-nologo')
    if ($filter) { $arguments += @('--filter', $filter) }
    & dotnet @arguments
}

$linuxRuns = $Linux -or $Set -eq 'full'
$sql = Get-Service 'MSSQL$SQLEXPRESS' -ErrorAction SilentlyContinue
$sqlStartedHere = $false
try {
    if ($Set -ne 'quick' -and $sql -and $sql.Status -ne 'Running') {
        # wt's comparison and the LINQ to SQL database tests (skipped without it).
        Start-Service 'MSSQL$SQLEXPRESS'; $sqlStartedHere = $true
    }

    if ($Set -eq 'runtime' -or $Set -eq 'full') {
        Step 'pack FrameworkOnCore packages (_feed)' { & "$wf4c\pack-frameworkoncore.ps1" -Build All } -Check throw
    }

    switch ($Set) {
        'quick' {
            Step 'FrameworkOnCore.Tests' { Test-Project 'FrameworkOnCore.Tests' }
            Step 'RuntimeUnitTests (Category!=Stress)' { Test-Project 'RuntimeUnitTests' 'Category!=Stress' }
            Step 'SqlClientTests' { Test-Project 'SqlClientTests' }
            Step 'translations\check.ps1' { & "$repo\translations\check.ps1" -Quiet }
        }
        'commit' {
            Step 'FrameworkOnCore.Tests' { Test-Project 'FrameworkOnCore.Tests' }
        }
        default {
            Step 'FrameworkOnCore.Tests' { Test-Project 'FrameworkOnCore.Tests' }
            Step 'RuntimeUnitTests' { Test-Project 'RuntimeUnitTests' }
            Step 'SqlClientTests' { Test-Project 'SqlClientTests' }
            Step 'translations\check.ps1' { & "$repo\translations\check.ps1" -Quiet }
        }
    }

    if ($Set -ne 'quick') {
        Step 'convert be, wt (against their baselines)' { & "$wf4c\convert-corpora.ps1" -Only be, wt } -Check throw
        Step 'be on Windows vs IIS' { & "$wf4c\verify-windows.ps1" -Name be }
        Step 'wt on Windows vs IIS' { & "$wf4c\verify-windows.ps1" -Name wt }
    }
    if ($Set -eq 'full') {
        Step 'wt every operation on Windows vs IIS' { & "$wf4c\verify-windows.ps1" -Name wt -Suite operations }
        Step 'be every operation on Windows vs IIS' { & "$wf4c\verify-windows.ps1" -Name be -Suite operations }
        Step 'culture matrix (Windows)' {
            & "$repo\tests\run-matrix.ps1" -NoBuild
            # It writes its table to the host; each failed test is a line of failures.txt.
            $failures = @(Get-Content "$repo\tests\TestResults\matrix\failures.txt" -ErrorAction SilentlyContinue | Where-Object { $_ })
            if ($failures.Count) { throw "$($failures.Count) failure(s): tests\TestResults\matrix\failures.txt" }
        } -Check throw
    }

    if ($linuxRuns -and $Set -ne 'quick') {
        try {
            docker info *> $null
            if ($LASTEXITCODE -ne 0) {
                Start-Process "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe"
                foreach ($i in 1..90) { Start-Sleep -Seconds 2; docker info *> $null; if ($LASTEXITCODE -eq 0) { break } }
                if ($LASTEXITCODE -ne 0) { throw 'Docker did not start' }
            }
            # Each exits non-zero on a failure (wt: its Linux-only error-page difference is documented in
            # corpora\parity\wt.allowed.json, so the expected state is 0). Continue: docker's stderr is not an error.
            Step 'Linux FrameworkOnCore.Tests, SqlClientTests' {
                $ErrorActionPreference = 'Continue'
                & "$repo\tests\FrameworkOnCore.Tests\run-tests-linux.ps1" -SqlServer
            } -Continue
            Step 'Linux RuntimeUnitTests' {
                $ErrorActionPreference = 'Continue'
                docker run --rm -m 6g -v "${repo}:/repo:ro" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh
            } -Continue
            Step 'be on Linux vs IIS' {
                & "$wf4c\run-linux.ps1" -App be\BlogEngine\BlogEngine.NET -Scenario corpora\regression\be.scenario.json -Golden corpora\parity\be.golden-webforms.json
            } -Continue
            Step 'wt on Linux vs IIS' {
                & "$wf4c\run-linux.ps1" -App wt\WingtipToys\WingtipToys -Scenario corpora\regression\wt.scenario.json -Golden corpora\parity\wt.golden-webforms.json -SqlServer
            } -Continue
            if ($Set -eq 'full') {
                Step 'be every operation on Linux vs IIS' {
                    & "$wf4c\run-linux.ps1" -App be\BlogEngine\BlogEngine.NET -Suite operations
                } -Continue
            }
        }
        finally {
            Write-Host '== stopping Docker Desktop, shutting WSL down'
            docker rm -f w2l-sql *> $null
            Get-Process 'Docker Desktop', 'com.docker.backend' -ErrorAction SilentlyContinue | Stop-Process -Force
            wsl --shutdown
        }
    }
}
finally {
    if ($sqlStartedHere) { Stop-Service 'MSSQL$SQLEXPRESS' -ErrorAction SilentlyContinue }
    Write-Host "`n===== $Set ($([int]((Get-Date) - $started).TotalMinutes) min)"
    $log | Out-Host
}
# The steps that went on after failing (-Continue): the set still fails.
if ($failedSteps.Count) { throw "failed: $($failedSteps -join ', ')" }
