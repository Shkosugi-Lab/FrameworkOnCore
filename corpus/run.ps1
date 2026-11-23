# The verification corpus (corpus\README.md): each probe of corpus\probes.json (samples\<name>, a .NET Framework web
# application whose pages are aspects to check) as experiments\wf4c\sample-parity.ps1 checks one: converted by the
# converter as a user converts it, run on Windows, or with -Linux in the container its Dockerfile makes (one at a time),
# its database reset where it has one, its parity scenario's snapshots compared with what the original answered on IIS
# (samples\<name>\golden-webforms.json). Then the conversion's decisions against the probe's baseline
# (corpus\baseline\<name>.json: the converter's snapshot command). The results: corpus\_out\RESULTS-<platform>.md.
#
#   .\corpus\run.ps1 [-Only PageProbe,AuthProbe] [-Linux] [-UpdateBaseline]
#
# It fails (exit 1) when a conversion fails, a snapshot differs from IIS's, or a conversion's decisions differ from the
# baseline (-UpdateBaseline records them, once looked at). corpus\catalog.py --check is run first: a probe or a snapshot
# without its IIS answers is not the corpus's.
# (the port the golden data was recorded on: a page that shows its URL shows it; one probe at a time)
param([string[]]$Only, [switch]$Linux, [switch]$UpdateBaseline, [int]$Port = 5096)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$out = Join-Path $PSScriptRoot '_out'
$baselines = Join-Path $PSScriptRoot 'baseline'
$parity = Join-Path $repo 'experiments\wf4c\sample-parity.ps1'
$converter = Join-Path $repo 'src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll'
$probes = (Get-Content (Join-Path $PSScriptRoot 'probes.json') -Raw -Encoding UTF8 | ConvertFrom-Json).probes
if ($Only) { $probes = @($probes | Where-Object { $Only -contains $_.name }) }
New-Item -ItemType Directory -Force $out, $baselines | Out-Null
# The culture profile of the machine the golden data was recorded on (corpus\culture-profile.json: ja-JP and en-US, its
# defaults ja-JP), for the probes without cultures.txt: the converter's own capture is its caller's UI language's (a
# shell's made it en-US where IIS's are ja-JP).
$env:FOC_CULTURE_PROFILE = Join-Path $PSScriptRoot 'culture-profile.json'
# The probes that name their cultures (cultures.txt): theirs, captured once (corpus\cultures\<probe>.json).
$env:FOC_CULTURE_PROFILES = Join-Path $PSScriptRoot 'cultures'
foreach ($name in 'DOTNET_CLI_UI_LANGUAGE', 'VSLANG') { Remove-Item "Env:$name" -ErrorAction SilentlyContinue }

& python (Join-Path $PSScriptRoot 'catalog.py') --check
if ($LASTEXITCODE -ne 0) { throw 'the corpus is not whole (corpus\catalog.py --check)' }

$platform = if ($Linux) { 'Linux' } else { 'Windows' }
$results = [ordered]@{}
$port = $Port
foreach ($probe in $probes) {
    $log = Join-Path $out "$($probe.name).$($platform.ToLowerInvariant()).log"
    $ErrorActionPreference = 'Continue'
    # The conversion's decisions, as it made them (before the application runs and writes in its folder)
    $holder = @{ Decisions = $null }
    $snapshotArguments = @('--source', (Join-Path $repo "samples\$($probe.name)"), '--baseline', (Join-Path $baselines "$($probe.name).json"),
        '--exit', 0, '--diff', (Join-Path $out "$($probe.name).decisions-diff.txt"))
    if ($UpdateBaseline) { $snapshotArguments += '--update' }
    $afterConvert = {
        param($converted)
        & dotnet $converter snapshot $converted @snapshotArguments | Out-Null
        $holder.Decisions = $UpdateBaseline -or $LASTEXITCODE -eq 0
    }.GetNewClosure()
    $arguments = @{ Name = $probe.name; Port = $port; AfterConvert = $afterConvert }
    if ($Linux) { $arguments.Linux = $true } else { $arguments.Windows = $true }
    # (the probe's own output only: one that throws has no result line, and is no "OK" of the probe before it)
    $text = ''
    try { $text = & $parity @arguments *>&1 | Out-String }
    catch { $text = "$($_.Exception.Message)`n" }
    $ErrorActionPreference = 'Stop'
    [IO.File]::WriteAllText($log, $text)
    $line = ($text -split "`n" | Where-Object { $_ -match '^RESULT:' } | Select-Object -Last 1)
    $run = if ($line -match '^RESULT: OK') { 'OK' } elseif ($line) { 'FAIL' } else { 'no result' }
    # Only the probe's known differences (probes.json's known: documented, with why): as expected.
    $differing = @($text -split "`n" | Where-Object { $_ -match '^\s+NG\s+(.+?)\s*$' } | ForEach-Object { $Matches[1] })
    if ($run -eq 'FAIL' -and $probe.known -and $differing.Count -gt 0 -and -not ($differing | Where-Object { @($probe.known.snapshots) + $(if ($Linux) { @($probe.known.linux) } else { @() }) -notcontains $_ })) {
        $run = 'OK'
        $line = "$line (known: $($differing -join ', '))"
    }
    # (without a result line: what went wrong, else the output's last line: "http://localhost:5096 did not answer")
    $detail = if ($line) { "$line".Trim() } else {
        $told = @($text -split "`n" | Where-Object { $_ -match 'Exception|failed|error' } | Select-Object -First 1) + @($text -split "`n" | Where-Object { $_.Trim() } | Select-Object -Last 1)
        ("$($told | Select-Object -First 1)" -replace '\s+', ' ').Trim()
    }

    $converted = Join-Path $repo "experiments\wf4c\_samples\$($probe.name)\converted"
    $decisions = $holder.Decisions
    $results[$probe.name] = [ordered]@{ Converted = Test-Path $converted; Decisions = $decisions; Run = $run; Detail = $detail }
    Write-Host ("{0}: {1} {2}" -f $probe.name, $run, $detail)
}

$lines = @("# The corpus's results ($platform, $(Get-Date -Format 'yyyy-MM-dd HH:mm'))", '',
    '| probe | converted | decisions = baseline | answers = IIS | |', '|---|---|---|---|---|')
$failed = 0
foreach ($probe in $probes) {
    $r = $results[$probe.name]
    $ok = $r.Converted -and $r.Decisions -and $r.Run -eq 'OK'
    if (-not $ok) { $failed++ }
    $lines += "| $($probe.name) | $(if ($r.Converted) { 'yes' } else { '**no**' }) | $(if ($r.Decisions) { 'yes' } else { '**no**' }) | $(if ($r.Run -eq 'OK') { 'yes' } else { "**$($r.Run)**" }) | $($r.Detail -replace '\|', '/') |"
}
$lines += '', "$($probes.Count - $failed) of $($probes.Count) probes as their baselines and IIS."
[IO.File]::WriteAllText((Join-Path $out "RESULTS-$($platform.ToLowerInvariant()).md"), ($lines -join "`n") + "`n", (New-Object System.Text.UTF8Encoding $false))
$lines[-1]
if ($failed -gt 0) { exit 1 }
