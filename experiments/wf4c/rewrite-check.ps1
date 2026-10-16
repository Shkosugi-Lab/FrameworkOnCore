# IIS's URL Rewrite module and the converted application's rewriting, answer by answer: the same requests to the sample
# (samples\RewriteProbe) on IIS (the recorded site, the module installed) and to the converted application on Windows,
# with each of UrlRewrite:Mode's values (-Modes; Iis: the host's IIS-compatible rewriting, AspNetCore: ASP.NET Core's
# rewriter, Off: no rule run). For each request, as sent (redirects not followed): the status, Location, what
# Show.aspx saw (its URLs, server variables, its form's action) and the W3C log's line (cs-uri-stem, cs-uri-query,
# sc-status, sc-substatus), the response's X-Probe header. The server's part as the original server's is imported: a
# global rule (applicationHost.config's globalRules, for this port only) and the site's allowedServerVariables are set
# on IIS while this runs, captured with capture-iis.ps1 into the converted application's iis-profile.config (the
# converter's --iis-profile). The bodies' text only with -CompareText: IIS's are in its language and detailed for a local
# client (its error pages), an unknown URL's 404 is empty here (httpErrors: not yet).
#   .\experiments\wf4c\rewrite-check.ps1 [-Name RewriteProbe] [-Modes Iis,AspNetCore,Off] [-CompareText]
param(
    [string]$Name = 'RewriteProbe',
    [int]$Port = 5096,
    [string[]]$Modes = @('Iis', 'AspNetCore'),
    [switch]$CompareText
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$site = Join-Path $work 'iis'
$converted = Join-Path $work 'converted'
foreach ($folder in $site, $converted) { if (-not (Test-Path $folder)) { throw "${folder}: sample-parity.ps1 -Name $Name -Record / -Windows first" } }
$base = "http://localhost:$Port"

$requests = @(
    '/products/5', '/products/5?x=1', '/products/x', '/old/7', '/old/7?k=v', '/temp', '/temp?k=v', '/LOWER/AbC', '/lower/abc',
    '/app/some/route', '/app/some/route?k=v', '/app/real.htm', '/app/', '/app', '/host-check', '/q?a=42&b=1', '/q?b=1',
    '/promo', '/promo?k=v', '/chain', '/scheme', '/blocked', '/abort', '/products/5%20x', '/Products/5', '/Show.aspx?id=9',
    '/files/a/b.txt', '/files/a', '/exact', '/Exact?k=v', '/exact/', '/any?x=5', '/any', '/track?n=8', '/track', '/vars?z=1', '/logged?k=v',
    '/see?k=v', '/tmp307?k=v', '/gone', '/sub/item/3', '/sub/item/3?k=v', '/sub/go', '/sub/old-page', '/sub/app/x', '/sub/', '/sub',
    '/setvars?a=1', '/denied', '/x/here', '/sub/x/here', '/global/abc', '/global/abc?k=v'
)

function Wait-Answer {
    foreach ($i in 1..600) { if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { return }; Start-Sleep -Milliseconds 200 }
    throw "$base did not answer"
}

function Send-All([string]$marker) {
    $answers = @()
    foreach ($path in $requests) {
        $headers = [IO.Path]::GetTempFileName()
        # IIS writes no line for an aborted request: its own User-Agent, out of the comparison of the logs.
        $agent = if ($path -eq '/abort') { 'rewrite-check/abort' } else { $marker }
        $body = (curl.exe -s -A $agent -D $headers --max-time 60 "$base$path") -join "`n"
        $exit = $LASTEXITCODE
        $head = Get-Content $headers -Raw
        Remove-Item $headers
        # The status line's reason too (IIS writes its own: "302 Redirect").
        $status = if ($head -match '^HTTP/\S+ (\d+[^\r\n]*)') { $Matches[1].Trim() } else { "(no answer: curl $exit)" }
        $location = if ($head -match '(?im)^Location: *(.*?)\r?$') { $Matches[1] } else { '-' }
        $probe = if ($head -match '(?im)^X-Probe: *(.*?)\r?$') { $Matches[1] } else { '-' }
        $urls = if ($body -match 'urls: <span[^>]*>(.*?)</span>') { [Net.WebUtility]::HtmlDecode($Matches[1]) } else { '-' }
        $action = if ($body -match '<form [^>]*action="([^"]*)"') { [Net.WebUtility]::HtmlDecode($Matches[1]) } else { '-' }
        $text = if ($urls -eq '-' -and $body) { ($body -replace '<[^>]*>', ' ' -replace '\s+', ' ').Trim() } else { '' }
        if ($text.Length -gt 120) { $text = $text.Substring(0, 120) }
        $answers += [pscustomobject]@{ Path = $path; Status = $status; Location = $location; Probe = $probe; Urls = $urls; Action = $action; Text = $text }
    }
    , $answers
}

# The lines of this run (its User-Agent) under their #Fields.
function Read-Log([string]$folder, [string]$marker) {
    $ua = $marker.Replace(' ', '+')
    $rows = @()
    foreach ($file in Get-ChildItem $folder -Filter 'u_ex*.log' -ErrorAction SilentlyContinue | Sort-Object LastWriteTime) {
        $fields = $null
        $stream = [IO.FileStream]::new($file.FullName, 'Open', 'Read', 'ReadWrite, Delete')
        $text = ([IO.StreamReader]::new($stream, [Text.Encoding]::UTF8)).ReadToEnd()
        $stream.Dispose()
        foreach ($line in $text -split "`r?`n") {
            if ($line.StartsWith('#Fields: ')) { $fields = $line.Substring(9).Split(' '); continue }
            if ($line.StartsWith('#') -or -not $fields -or $line.IndexOf($ua) -lt 0) { continue }
            $values = $line.Split(' ')
            $row = @{}
            for ($i = 0; $i -lt $fields.Length; $i++) { $row[$fields[$i]] = $values[$i] }
            $rows += "$($row['cs-uri-stem']) $($row['cs-uri-query']) $($row['sc-status']).$($row['sc-substatus'])"
        }
    }
    , $rows
}

function New-Marker { 'rewrite-check/1.0 (' + [Guid]::NewGuid().ToString('N').Substring(0, 8) + ')' }

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
$siteId = (& $appcmd list site $pool /text:id)
# The server's global rule (this port's requests only) and the variables the site lets rules set.
$global = 'frameworkoncore-rewrite-check'
& $appcmd set config -section:system.webServer/rewrite/globalRules /-"[name='$global']" /commit:apphost 2>&1 | Out-Null
& $appcmd set config -section:system.webServer/rewrite/globalRules /+"[name='$global',stopProcessing='true']" /commit:apphost | Out-Null
& $appcmd set config -section:system.webServer/rewrite/globalRules "/[name='$global'].match.url:^global/(.*)$" /commit:apphost | Out-Null
& $appcmd set config -section:system.webServer/rewrite/globalRules /+"[name='$global'].conditions.[input='{SERVER_PORT}',pattern='^$Port$']" /commit:apphost | Out-Null
& $appcmd set config -section:system.webServer/rewrite/globalRules "/[name='$global'].action.type:Rewrite" "/[name='$global'].action.url:/Show.aspx?global={R:1}" /commit:apphost | Out-Null
foreach ($variable in 'PROBE_VAR', 'HTTP_X_PROBE', 'RESPONSE_X_Probe') {
    & $appcmd set config $pool -section:system.webServer/rewrite/allowedServerVariables /+"[name='$variable']" /commit:apphost | Out-Null
}
$marker = New-Marker
try {
    & (Join-Path $PSScriptRoot 'capture-iis.ps1') -Site $pool -Out (Join-Path $converted 'iis-profile.config') | Out-Null
    Wait-Answer
    $iis = Send-All $marker
    netsh http flush logbuffer | Out-Null
    Start-Sleep 2
    $iisLog = Read-Log (Join-Path $env:SystemDrive "inetpub\logs\LogFiles\W3SVC$siteId") $marker
}
finally {
    & $appcmd set config -section:system.webServer/rewrite/globalRules /-"[name='$global']" /commit:apphost 2>&1 | Out-Null
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
}

foreach ($mode in $Modes) {
    $logs = Join-Path $work "rewrite-log-$mode"
    Remove-Item $logs -Recurse -Force -ErrorAction SilentlyContinue
    $env:ASPNETCORE_URLS = $base
    $env:W3CLogging__LogDirectory = $logs
    $env:UrlRewrite__Mode = $mode
    $marker = New-Marker
    $errors = Join-Path $work "rewrite-err-$mode.log"
    $process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
        -RedirectStandardError $errors -RedirectStandardOutput (Join-Path $work "rewrite-out-$mode.log")
    $started = $true
    try {
        foreach ($i in 1..600) {
            if ($process.HasExited) { $started = $false; break }
            if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { break }
            Start-Sleep -Milliseconds 200
        }
        if ($started) {
            $ours = Send-All $marker
            Start-Sleep 2
            $oursLog = Read-Log $logs $marker
        }
    }
    finally {
        # The worker process too (WebFormsProcess), and the port free before the next run.
        if (-not $process.HasExited) { try { taskkill /PID $process.Id /T /F 2>&1 | Out-Null } catch { } }
        foreach ($i in 1..100) { if (-not (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue)) { break }; Start-Sleep -Milliseconds 200 }
        Remove-Item Env:W3CLogging__LogDirectory, Env:UrlRewrite__Mode
    }

    "===== UrlRewrite:Mode $mode"
    # The application did not start (ASP.NET Core's rewriter refuses rules it cannot read): its error.
    if (-not $started) {
        "  the application did not start:"
        Get-Content $errors | Select-Object -First 3 | ForEach-Object { "    $_" }
        "RESULT ${mode}: 0 of $($requests.Count) answers (not started)"
        continue
    }
    $same = 0
    for ($i = 0; $i -lt $requests.Count; $i++) {
        $a = $iis[$i]; $b = $ours[$i]
        $differences = @()
        foreach ($field in @('Status', 'Location', 'Probe', 'Urls', 'Action') + $(if ($CompareText) { 'Text' })) {
            if ($a.$field -ne $b.$field) { $differences += "$field`n           IIS       $($a.$field)`n           converted $($b.$field)" }
        }
        $logged = @($requests[0..$i] | Where-Object { $_ -ne '/abort' }).Count - 1
        $logA = if ($a.Path -eq '/abort') { '' } elseif ($logged -lt $iisLog.Count) { $iisLog[$logged] } else { '(none)' }
        $logB = if ($a.Path -eq '/abort') { '' } elseif ($logged -lt $oursLog.Count) { $oursLog[$logged] } else { '(none)' }
        if ($logA -ne $logB) { $differences += "log`n           IIS       $logA`n           converted $logB" }
        if ($differences.Count -eq 0) { $same++; "  OK   $($a.Path) -> $($a.Status) $(if ($a.Location -ne '-') { $a.Location })" }
        else { "  NG   $($a.Path)"; $differences | ForEach-Object { "         $_" } }
    }
    "RESULT ${mode}: $same of $($requests.Count) answers the same"
}
