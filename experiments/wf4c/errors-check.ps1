# IIS's error pages (system.webServer/httpErrors) against the converted application's, answer by answer: the same requests
# to the sample (samples\ErrorsProbe) on IIS (the recorded site) and to the converted application on Windows. For each
# request, as sent (redirects not followed): the status line, Location, Content-Type, every X- header and Cache-Control,
# which body it is (the application's, IIS's custom page and its heading, IIS's detailed page, ASP.NET's error page,
# the application's error page or file, none) and the W3C log's line.
#   .\experiments\wf4c\errors-check.ps1 [-Name ErrorsProbe]
param(
    [string]$Name = 'ErrorsProbe',
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
    '/nothing.htm', '/nothing', '/App.aspx', '/App.aspx?status=404', '/App.aspx?status=404&skip=1', '/App.aspx?status=404&nobody=1',
    '/App.aspx?status=500', '/App.aspx?status=400', '/App.aspx?status=503', '/App.aspx?status=410', '/App.aspx?status=401',
    '/App.aspx?throw=1', '/App.aspx?http404=1', '/nothing.aspx', '/web.config', '/bin/x.dll',
    '/custom/nothing.htm', '/custom/nothing.htm?k=v', '/custom/App.aspx?status=404', '/custom/App.aspx?status=500', '/custom/App.aspx?throw=1',
    '/custom/App.aspx?status=403', '/custom/',
    '/pass/App.aspx?status=404', '/pass/App.aspx?status=404&nobody=1', '/pass/nothing.htm', '/pass/App.aspx?throw=1',
    '/replace/App.aspx?status=404&skip=1', '/replace/App.aspx?throw=1',
    '/detailed/nothing.htm', '/detailed/App.aspx?status=404', '/detailed/App.aspx?throw=1'
)

function Wait-Answer {
    foreach ($i in 1..600) { if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { return }; Start-Sleep -Milliseconds 200 }
    throw "$base did not answer"
}

# Which body: the application's, IIS's (its custom page's heading, its detailed page's), ASP.NET's error page, the
# application's error page or file.
function Kind([string]$body) {
    $text = ($body -replace '<[^>]*>', ' ' -replace '\s+', ' ').Trim()
    if ($body.Length -eq 0) { return 'empty' }
    if ($body -match 'APP BODY \d+') { return "app: $($Matches[0])" }
    if ($body -match 'CUSTOM 404 PAGE') { return "execute-url: $text" }
    if ($body -match 'FILE 500 PAGE') { return 'file: errors/500.htm' }
    if ($body -match '<div id="header"><h1>Server Error</h1>' -and $body -match '<h2>(.*?)</h2>') { return "iis-custom: $($Matches[1])" }
    if ($body -match '<title>(IIS 10\.0 [^<]*)</title>') { return "iis-detailed: $($Matches[1])" }
    if ($body -match "Server Error in '/' Application" -and $body -match '<i>(.*?)</i>') { return "aspnet: $($Matches[1])" }
    if ($body -match 'APP PAGE') { return 'app page' }
    return "other: $($text.Substring(0, [Math]::Min(100, $text.Length)))"
}

function Send-All([string]$marker) {
    $answers = @()
    foreach ($request in $requests) {
        $path, $kind = $request.Split('|')
        $bodyFile = [IO.Path]::GetTempFileName()
        $arguments = @('-s', '-o', $bodyFile, '-A', $marker, '--max-time', '60')
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
        $type = if ($head -match '(?im)^Content-Type: *(.*?)
?$') { $Matches[1] } else { '-' }
        $body = [IO.File]::ReadAllText($bodyFile, [Text.Encoding]::UTF8)
        Remove-Item $bodyFile
        $answers += [pscustomobject]@{ Path = $request; Status = $status; Location = $location; Type = $type; Headers = $x; Body = (Kind $body) }
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

function New-Marker { 'errors-check/1.0 (' + [Guid]::NewGuid().ToString('N').Substring(0, 8) + ')' }

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

$logs = Join-Path $work 'errors-log'
Remove-Item $logs -Recurse -Force -ErrorAction SilentlyContinue
$env:ASPNETCORE_URLS = $base
$env:W3CLogging__LogDirectory = $logs
$marker = New-Marker
$process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden `
    -RedirectStandardError (Join-Path $work 'errors-err.log') -RedirectStandardOutput (Join-Path $work 'errors-out.log')
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
    foreach ($field in 'Status', 'Location', 'Type', 'Headers', 'Body') {
        if ($a.$field -ne $b.$field) { $differences += "$field`n           IIS       $($a.$field)`n           converted $($b.$field)" }
    }
    $logA = if ($i -lt $iisLog.Count) { $iisLog[$i] } else { '(none)' }
    $logB = if ($i -lt $oursLog.Count) { $oursLog[$i] } else { '(none)' }
    if ($logA -ne $logB) { $differences += "log`n           IIS       $logA`n           converted $logB" }
    if ($differences.Count -eq 0) { $same++; "  OK   $($a.Path) -> $($a.Status) $(if ($a.Location -ne '-') { $a.Location }) | $($a.Body)" }
    else { "  NG   $($a.Path)"; $differences | ForEach-Object { "         $_" } }
}
"RESULT: $same of $($requests.Count) answers the same"
