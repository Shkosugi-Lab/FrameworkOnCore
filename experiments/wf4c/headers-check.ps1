# IIS's response headers (customHeaders, redirectHeaders) and HTTP redirection (httpRedirect) against the converted
# application's, answer by answer: the same requests to the sample (samples\HeadersProbe) on IIS (the recorded site,
# its HTTP Redirection feature installed) and to the converted application on Windows. For each request, as sent
# (redirects not followed): the status line, Location, every X- header (each line: a header sent twice is two) and
# Cache-Control, the W3C log's line (cs-uri-stem, cs-uri-query, sc-status, sc-substatus). The server's part
# (applicationHost.config's customHeaders: X-Powered-By) imported as from the original server: capture-iis.ps1 into the
# converted application's iis-profile.config (the converter's --iis-profile); -NoProfile leaves it out (IIS's defaults).
#   .\experiments\wf4c\headers-check.ps1 [-Name HeadersProbe] [-NoProfile]
param(
    [string]$Name = 'HeadersProbe',
    [int]$Port = 5096,
    [switch]$NoProfile
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$site = Join-Path $work 'iis'
$converted = Join-Path $work 'converted'
foreach ($folder in $site, $converted) { if (-not (Test-Path $folder)) { throw "${folder}: sample-parity.ps1 -Name $Name -Record / -Windows first" } }
$base = "http://localhost:$Port"

# A request: its path, and a request header ('cond': If-Modified-Since with the file's own Last-Modified: 304) or a body
# ('post200': a POST of 200 bytes).
$requests = @(
    '/a.htm', '/a.htm|cond', '/Show.aspx', '/Show.aspx?set=1', '/Show.aspx?redirect=1', '/Show.aspx?error=1', '/Show.aspx?status=404',
    '/nothing.htm', '/nothing', '/web.config', '/new', '/secure/s.htm', '/secure/', '/secure/nothing.htm', '/loc/l.htm', '/loc/none',
    '/old', '/old/', '/old/o.htm', '/old/x.htm?q=1', '/old/sub/y.aspx?q=1', '/old/keep/k.htm', '/exact/p/q.htm?z=1', '/exact',
    '/child', '/child/', '/child/c.htm', '/child/d/e.htm?q=1', '/wild/x.php', '/wild/x.php?q=1', '/wild/x.htm', '/wild/a/b/c',
    '/ext/z?q=1', '/Moved.aspx?k=v', '/moved.aspx', '/old/rw', '/tonew',
    '/wild2/page.htm', '/wild2/sub/page.htm', '/wild2/full/x', '/wild2/full/y/z.htm', '/wild2/data_in.asp?k=v', '/wild2/d/e.aspx',
    '/wild2/e.aspx', '/wild2/other.txt', '/base/g.htm', '/base/deep/f.htm', '/base/deep/more/h.htm', '/pq/a?b=2', '/pq/a',
    '/vnon/a.htm?k=v', '/perm8/x', '/rel/x.htm',
    '/docs/', '/docs', '/browse/', '/upload/u.htm', '/upload/u.htm?0123456789abc', '/upload/Show.aspx|post200', '/Show.aspx|post200'
)

function Wait-Answer {
    foreach ($i in 1..600) { if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { return }; Start-Sleep -Milliseconds 200 }
    throw "$base did not answer"
}

function Send-All([string]$marker) {
    $answers = @()
    foreach ($request in $requests) {
        $path, $kind = $request.Split('|')
        $arguments = @('-s', '-o', 'NUL', '-A', $marker, '--max-time', '60')
        if ($kind -eq 'cond') {
            $modified = (curl.exe -s -I -A $marker "$base$path") | Where-Object { $_ -match '^Last-Modified: *(.*)$' } | ForEach-Object { $Matches[1].Trim() } | Select-Object -First 1
            $arguments += @('-H', "If-Modified-Since: $modified")
        }
        if ($kind -eq 'post200') { $arguments += @('--data', ('x=' + ('y' * 198))) }
        $headers = [IO.Path]::GetTempFileName()
        curl.exe @arguments -D $headers "$base$path" | Out-Null
        $head = (Get-Content $headers) -join "`n"
        Remove-Item $headers
        # The status line's reason too (IIS's documentation: "302 Redirect", "307 Redirect").
        $status = if ($head -match '^HTTP/\S+ (\d+[^

]*)') { $Matches[1].Trim() } else { '(no answer)' }
        $location = if ($head -match '(?im)^Location: *(.*?)\r?$') { $Matches[1] } else { '-' }
        $x = @($head -split "`n" | Where-Object { $_ -match '^(X-[^:]+|Cache-Control): *(.*?)\r?$' } | ForEach-Object { $_.Trim() } | Sort-Object) -join '; '
        $answers += [pscustomobject]@{ Path = $request; Status = $status; Location = $location; Headers = $x }
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
            $rows += "$($row['cs-method']) $($row['cs-uri-stem']) $($row['cs-uri-query']) $($row['sc-status']).$($row['sc-substatus'])"
        }
    }
    # The HEAD requests that read Last-Modified are not compared.
    , @($rows | Where-Object { -not $_.StartsWith('HEAD ') })
}

function New-Marker { 'headers-check/1.0 (' + [Guid]::NewGuid().ToString('N').Substring(0, 8) + ')' }

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
$marker = New-Marker
$profileFile = Join-Path $converted 'iis-profile.config'
try {
    if ($NoProfile) { Remove-Item $profileFile -ErrorAction SilentlyContinue }
    else { & (Join-Path $PSScriptRoot 'capture-iis.ps1') -Site $pool -Out $profileFile | Out-Null }
    Wait-Answer
    $iis = Send-All $marker
    netsh http flush logbuffer | Out-Null
    Start-Sleep 2
    $iisLog = Read-Log (Join-Path $env:SystemDrive "inetpub\logs\LogFiles\W3SVC$siteId") $marker
}
finally {
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
}

$logs = Join-Path $work 'headers-log'
Remove-Item $logs -Recurse -Force -ErrorAction SilentlyContinue
$env:ASPNETCORE_URLS = $base
$env:W3CLogging__LogDirectory = $logs
$marker = New-Marker
$process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
    -RedirectStandardError (Join-Path $work 'headers-err.log') -RedirectStandardOutput (Join-Path $work 'headers-out.log')
try {
    Wait-Answer
    $ours = Send-All $marker
    Start-Sleep 2
    $oursLog = Read-Log $logs $marker
}
finally {
    # The worker process too (WebFormsProcess), and the port free afterwards.
    if (-not $process.HasExited) { try { taskkill /PID $process.Id /T /F 2>&1 | Out-Null } catch { } }
    foreach ($i in 1..100) { if (-not (Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue)) { break }; Start-Sleep -Milliseconds 200 }
    Remove-Item Env:W3CLogging__LogDirectory
}

$same = 0
for ($i = 0; $i -lt $requests.Count; $i++) {
    $a = $iis[$i]; $b = $ours[$i]
    $differences = @()
    foreach ($field in 'Status', 'Location', 'Headers') {
        if ($a.$field -ne $b.$field) { $differences += "$field`n           IIS       $($a.$field)`n           converted $($b.$field)" }
    }
    $logA = if ($i -lt $iisLog.Count) { $iisLog[$i] } else { '(none)' }
    $logB = if ($i -lt $oursLog.Count) { $oursLog[$i] } else { '(none)' }
    if ($logA -ne $logB) { $differences += "log`n           IIS       $logA`n           converted $logB" }
    if ($differences.Count -eq 0) { $same++; "  OK   $($a.Path) -> $($a.Status) $(if ($a.Location -ne '-') { $a.Location }) [$($a.Headers)]" }
    else { "  NG   $($a.Path)"; $differences | ForEach-Object { "         $_" } }
}
"RESULT: $same of $($requests.Count) answers the same"
