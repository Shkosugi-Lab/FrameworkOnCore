# IIS's compression (system.webServer/httpCompression, urlCompression) against the converted application's: the same
# requests, in the same order, to the sample (samples\HeadersProbe) on IIS (the recorded site; its static compression,
# Windows' default feature) and to the converted application on Windows. For each: the status line, Content-Encoding,
# Content-Length (a compressed body's: only that it is one, gzip's bytes are each implementation's), Transfer-Encoding,
# Vary, ETag's form (the file's tag, with a suffix or not). The hits matter: IIS compresses a static file once it is asked
# for twice within 10 seconds (frequentHitThreshold), so the order is the same on both. The server's compression (its
# modules, httpCompression) imported as from the original server: capture-iis.ps1 into the converted application's
# iis-profile.config; -NoProfile leaves it out (Windows' default: static compression only).
#   .\experiments\wf4c\compression-check.ps1 [-Name HeadersProbe]
param(
    [string]$Name = 'HeadersProbe',
    [int]$Port = 5096,
    [switch]$NoProfile
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$site = Join-Path $work 'iis'
$converted = Join-Path $work 'converted'
$base = "http://localhost:$Port"

# A request: what it shows, its path, the arguments its curl has.
$gzip = @('-H', 'Accept-Encoding: gzip')
$requests = @(
    @{ Label = 'big.htm 1st'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: gzip, deflate, br') },
    @{ Label = 'big.htm 2nd'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: gzip, deflate, br') },
    @{ Label = 'big.htm 3rd'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: gzip, deflate, br') },
    @{ Label = 'big.htm HTTP/1.0'; Path = '/big.htm'; Args = @('--http1.0') + $gzip },
    @{ Label = 'big.htm via a proxy'; Path = '/big.htm'; Args = $gzip + @('-H', 'Via: 1.1 proxy') },
    @{ Label = 'big.htm a range'; Path = '/big.htm'; Args = $gzip + @('-H', 'Range: bytes=0-99') },
    @{ Label = 'big.htm no Accept-Encoding'; Path = '/big.htm'; Args = @() },
    @{ Label = 'big.htm deflate only'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: deflate') },
    @{ Label = 'big.htm br only'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: br') },
    @{ Label = 'big.htm gzip;q=0'; Path = '/big.htm'; Args = @('-H', 'Accept-Encoding: gzip;q=0, deflate') },
    @{ Label = 'small.htm 1st'; Path = '/small.htm'; Args = $gzip },
    @{ Label = 'small.htm 2nd'; Path = '/small.htm'; Args = $gzip },
    @{ Label = 'big.js 1st'; Path = '/big.js'; Args = $gzip },
    @{ Label = 'big.js 2nd'; Path = '/big.js'; Args = $gzip },
    @{ Label = 'big.png 1st'; Path = '/big.png'; Args = $gzip },
    @{ Label = 'big.png 2nd'; Path = '/big.png'; Args = $gzip },
    @{ Label = 'Show.aspx'; Path = '/Show.aspx'; Args = $gzip },
    @{ Label = 'Show.aspx 2nd'; Path = '/Show.aspx'; Args = $gzip },
    @{ Label = 'Show.aspx no Accept-Encoding'; Path = '/Show.aspx'; Args = @() },
    @{ Label = 'Show.aspx HTTP/1.0'; Path = '/Show.aspx'; Args = @('--http1.0') + $gzip },
    @{ Label = 'Show.aspx via a proxy'; Path = '/Show.aspx'; Args = $gzip + @('-H', 'Via: 1.1 proxy') },
    @{ Label = 'Show.aspx a redirect'; Path = '/Show.aspx?redirect=1'; Args = $gzip },
    @{ Label = 'Show.aspx an error'; Path = '/Show.aspx?error=1'; Args = $gzip },
    @{ Label = 'Show.aspx 404 of its own'; Path = '/Show.aspx?status=404'; Args = $gzip },
    @{ Label = 'Check.htm (a static file) 1st'; Path = '/Check.htm'; Args = $gzip },
    @{ Label = 'nothing.htm'; Path = '/nothing.htm'; Args = $gzip },
    @{ Label = 'nothing.htm 2nd'; Path = '/nothing.htm'; Args = $gzip },
    @{ Label = 'browse/ (a listing)'; Path = '/browse/'; Args = $gzip },
    @{ Label = 'old/o.htm (a redirect)'; Path = '/old/o.htm'; Args = $gzip }
)

function Wait-Answer {
    foreach ($i in 1..600) { if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { return }; Start-Sleep -Milliseconds 200 }
    throw "$base did not answer"
}

function Pick([string]$head, [string]$name) {
    if ($head -match "(?im)^${name}: *(.*?)\r?$") { $Matches[1] } else { '-' }
}

function Send-All {
    $answers = @()
    foreach ($r in $requests) {
        $arguments = @('-s', '-D', '-', '-o', 'NUL') + $r.Args + @("$base$($r.Path)")
        $head = (curl.exe @arguments) -join "`n"
        $status = if ($head -match '^HTTP/\S+ (\d+[^\n]*)') { $Matches[1].Trim() } else { '(no answer)' }
        $etag = Pick $head 'ETag'
        # The tag's form: the file's own ("...:0"), or with a suffix, not its value (the copies' times differ).
        $etagForm = if ($etag -eq '-') { '-' } elseif ($etag -match ':0"$') { 'file' } else { $etag -replace '^"[0-9a-f]+', '"<time>' }
        $encoding = Pick $head 'Content-Encoding'
        $length = if ($encoding -ne '-' -and (Pick $head 'Content-Length') -ne '-') { 'compressed' } else { Pick $head 'Content-Length' }
        $answers += [pscustomobject]@{ Label = $r.Label; Status = $status; Encoding = $encoding; Length = $length; Chunked = (Pick $head 'Transfer-Encoding'); Vary = (Pick $head 'Vary'); ETag = $etagForm }
    }
    , $answers
}

# IIS
$appcmd = Join-Path $env:SystemRoot 'System32\inetsrv\appcmd.exe'
$pool = "frameworkoncore-$($Name.ToLowerInvariant())"
& $appcmd delete site $pool 2>&1 | Out-Null
& $appcmd delete apppool $pool 2>&1 | Out-Null
& $appcmd add apppool /name:$pool /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated | Out-Null
& $appcmd add site /name:$pool /physicalPath:$site ("/bindings:http/*:{0}:" -f $Port) | Out-Null
& $appcmd set app "$pool/" /applicationPool:$pool | Out-Null
& $appcmd set config "$pool/" /section:anonymousAuthentication /userName:"" /commit:apphost | Out-Null
& icacls $site /grant "IIS AppPool\${pool}:(OI)(CI)(M)" /T /Q | Out-Null
$profileFile = Join-Path $converted 'iis-profile.config'
try {
    if ($NoProfile) { Remove-Item $profileFile -ErrorAction SilentlyContinue }
    else { & (Join-Path $PSScriptRoot 'capture-iis.ps1') -Site $pool -Out $profileFile | Out-Null }
    Wait-Answer
    $iis = Send-All
}
finally {
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
}

$env:ASPNETCORE_URLS = $base
$process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden
try {
    Wait-Answer
    $ours = Send-All
}
finally {
    if (-not $process.HasExited) { try { taskkill /PID $process.Id /T /F 2>&1 | Out-Null } catch { } }
    foreach ($i in 1..100) { if (-not (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue)) { break }; Start-Sleep -Milliseconds 200 }
}

$same = 0
for ($i = 0; $i -lt $requests.Count; $i++) {
    $a = $iis[$i]; $b = $ours[$i]
    $differences = @()
    foreach ($field in 'Status', 'Encoding', 'Length', 'Chunked', 'Vary', 'ETag') {
        if ($a.$field -ne $b.$field) { $differences += "$field IIS '$($a.$field)' / converted '$($b.$field)'" }
    }
    if ($differences.Count -eq 0) { $same++; "  OK   $($a.Label): $($a.Status) | encoding $($a.Encoding) | length $($a.Length) | chunked $($a.Chunked) | vary $($a.Vary) | etag $($a.ETag)" }
    else { "  NG   $($a.Label)"; $differences | ForEach-Object { "         $_" } }
}
"RESULT: $same of $($requests.Count) answers the same"
