# IIS's W3C log and the converted application's (IisLogMiddleware, W3CLogging:Mode Iis), line by line: the same requests
# to the sample on IIS (the recorded site, as perf-check.ps1 runs it) and to the converted application on Windows, each
# log's lines of this run (the User-Agent marks them), compared field by field (date, time and time-taken are the
# moments', not compared; sc-bytes and cs-bytes shown side by side: the headers differ, Server: Microsoft-IIS/10.0 and
# Kestrel's...).
#   .\experiments\wf4c\iislog-check.ps1 [-Name PipelineProbe] [-Port 5096]
param(
    [string]$Name = 'PipelineProbe',
    [int]$Port = 5096
)

$ErrorActionPreference = 'Stop'
$work = Join-Path $PSScriptRoot "_samples\$Name"
$site = Join-Path $work 'iis'
$converted = Join-Path $work 'converted'
foreach ($folder in $site, $converted) { if (-not (Test-Path $folder)) { throw "${folder}: sample-parity.ps1 -Name $Name -Record / -Windows first" } }
$marker = 'iislog-check/1.0 (' + [Guid]::NewGuid().ToString('N').Substring(0, 8) + ')'
$base = "http://127.0.0.1:$Port"
$longQuery = 'q=' + ('x' * 2100)

# A file of a type IIS's MIME map does not have (404.3), in both sites while this runs.
$probeFile = 'iislog-probe.xyz'
foreach ($folder in $site, $converted) { Set-Content (Join-Path $folder $probeFile) 'probe' -NoNewline }

$requests = @(
    @{ Path = '/Check.htm' },
    @{ Path = '/Events.aspx' },
    @{ Path = '/Events.aspx?a=1&b=two%20words' },
    @{ Path = '/Check.htm'; Referer = "$base/Events.aspx" },
    @{ Path = '/nothing.htm' },
    @{ Path = "/$probeFile" },
    @{ Path = '/web.config' },
    @{ Path = '/bin/x.dll' },
    @{ Path = '/App_Data/x.txt' },
    @{ Path = '/Global.asax.cs' },
    @{ Path = "/Events.aspx?$longQuery" },
    @{ Path = '/Events.aspx'; Method = 'POST'; Body = 'x=1&y=2' },
    @{ Path = '/Check.htm'; Method = 'HEAD' },
    @{ Path = '/Check.htm'; Header = 'If-Modified-Since: Sat, 01 Jan 2050 00:00:00 GMT' },
    @{ Path = '/Errors' },
    @{ Path = '/a%20b.htm' },
    @{ Path = '/%E6%97%A5%E6%9C%AC.htm' },
    @{ Path = '/' }
)

function Send-All {
    foreach ($r in $requests) {
        $arguments = @('-s', '-o', 'NUL', '-A', $marker)
        if ($r.Method -eq 'HEAD') { $arguments += '-I' } elseif ($r.Method) { $arguments += @('-X', $r.Method) }
        if ($r.Body) { $arguments += @('--data', $r.Body) }
        if ($r.Referer) { $arguments += @('-e', $r.Referer) }
        if ($r.Header) { $arguments += @('-H', $r.Header) }
        $arguments += "$base$($r.Path)"
        curl.exe @arguments | Out-Null
    }
}

function Wait-Answer {
    foreach ($i in 1..600) { if ((curl.exe -s -o NUL -w "%{http_code}" --max-time 120 "$base/Check.htm") -eq '200') { return }; Start-Sleep -Milliseconds 200 }
    throw "$base did not answer"
}

# The lines of this run (its User-Agent, as the log writes it) under their #Fields.
function Read-Log([string]$folder) {
    $ua = $marker.Replace(' ', '+')
    $rows = @()
    foreach ($file in Get-ChildItem $folder -Filter 'u_ex*.log' | Sort-Object LastWriteTime) {
        $fields = $null
        # IIS holds its file open: read it shared.
        $stream = [IO.FileStream]::new($file.FullName, 'Open', 'Read', 'ReadWrite, Delete')
        $text = ([IO.StreamReader]::new($stream, [Text.Encoding]::UTF8)).ReadToEnd()
        $stream.Dispose()
        foreach ($line in $text -split "`r?`n") {
            if ($line.StartsWith('#Fields: ')) { $fields = $line.Substring(9).Split(' '); continue }
            if ($line.StartsWith('#') -or -not $fields -or $line.IndexOf($ua) -lt 0) { continue }
            $values = $line.Split(' ')
            $row = [ordered]@{}
            for ($i = 0; $i -lt $fields.Length; $i++) { $row[$fields[$i]] = $values[$i] }
            $rows += [pscustomobject]$row
        }
    }
    , $rows
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
$siteId = (& $appcmd list site $pool /text:id)
try {
    Wait-Answer
    Send-All
    netsh http flush logbuffer | Out-Null
    Start-Sleep 2
    $iis = Read-Log (Join-Path $env:SystemDrive "inetpub\logs\LogFiles\W3SVC$siteId")
}
finally {
    & $appcmd delete site $pool 2>&1 | Out-Null
    & $appcmd delete apppool $pool 2>&1 | Out-Null
}

# The converted application
$logs = Join-Path $work 'iislog'
Remove-Item $logs -Recurse -Force -ErrorAction SilentlyContinue
$env:ASPNETCORE_URLS = $base
$env:W3CLogging__LogDirectory = $logs
$env:W3CLogging__Mode = 'Iis'
$process = Start-Process dotnet -ArgumentList "bin\$Name.dll" -WorkingDirectory $converted -PassThru -WindowStyle Hidden
try {
    Wait-Answer
    Send-All
    Start-Sleep 2
    $ours = Read-Log $logs
}
finally {
    # The worker process too (WebFormsProcess).
    try { taskkill /PID $process.Id /T /F 2>&1 | Out-Null } catch { }
    Remove-Item Env:W3CLogging__LogDirectory, Env:W3CLogging__Mode
    foreach ($folder in $site, $converted) { Remove-Item (Join-Path $folder $probeFile) -ErrorAction SilentlyContinue }
}

"IIS: $($iis.Count) lines, converted: $($ours.Count) lines (requests: $($requests.Count))"
$skip = 'date', 'time', 'time-taken', 'sc-bytes', 'cs-bytes'
$same = 0
for ($i = 0; $i -lt [Math]::Max($iis.Count, $ours.Count); $i++) {
    $a = if ($i -lt $iis.Count) { $iis[$i] } else { $null }
    $b = if ($i -lt $ours.Count) { $ours[$i] } else { $null }
    $differences = @()
    $names = @(); if ($a) { $names += $a.PSObject.Properties.Name }; if ($b) { $names += $b.PSObject.Properties.Name }
    foreach ($field in ($names | Select-Object -Unique)) {
        if ($skip -contains $field) { continue }
        $x = if ($a) { $a.$field } else { '(none)' }
        $y = if ($b) { $b.$field } else { '(none)' }
        if ($x -ne $y) { $differences += "$field IIS '$x' / converted '$y'" }
    }
    $label = if ($a) { "$($a.'cs-method') $($a.'cs-uri-stem')" } else { "$($b.'cs-method') $($b.'cs-uri-stem')" }
    $bytes = "sc-bytes $($a.'sc-bytes')/$($b.'sc-bytes') cs-bytes $($a.'cs-bytes')/$($b.'cs-bytes')"
    if ($differences.Count -eq 0) { $same++; "  OK   $label ($bytes)" } else { "  NG   $label ($bytes)"; $differences | ForEach-Object { "         $_" } }
}
"RESULT: $same of $([Math]::Max($iis.Count, $ours.Count)) lines the same (date, time, time-taken, bytes not compared)"
