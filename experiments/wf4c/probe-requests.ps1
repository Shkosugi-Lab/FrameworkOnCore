# Starts the converted RuntimeProbe (samples\RuntimeProbe, converted to -Dir) and requests its pages: on Windows, or with
# -Linux in the ASP.NET runtime image, in the culture deploy's start.sh sets (LANG: the culture profile's).
#   dotnet src\FrameworkOnCore.Converter\bin\Debug\net10.0\FrameworkOnCore.Converter.dll samples\RuntimeProbe\RuntimeProbe.csproj `
#       --out experiments\wf4c\probe --root samples\RuntimeProbe --culture-profile experiments\wf4c\_culture\culture-profile.json
#   .\experiments\wf4c\probe-requests.ps1 [-Linux]
param([string]$Dir = 'experiments\wf4c\probe', [int]$Port = 5188, [switch]$Linux, [string]$Lang = 'ja_JP.UTF-8')

$container = 'w2l-probe'
if ($Linux) {
    docker container rm --force $container 2>$null | Out-Null
    $mount = "$((Resolve-Path $Dir).Path):/app"
    docker run -d --name $container -p "${Port}:8080" -e LANG=$Lang -e ASPNETCORE_URLS=http://+:8080 -v $mount -w /app `
        mcr.microsoft.com/dotnet/aspnet:10.0 dotnet bin/RuntimeProbe.dll | Out-Null
}
else {
    $env:ASPNETCORE_URLS = "http://localhost:$Port"
    $process = Start-Process dotnet -ArgumentList 'bin\RuntimeProbe.dll' -WorkingDirectory $Dir -PassThru -WindowStyle Hidden `
        -RedirectStandardOutput "$env:TEMP\probe-out.log" -RedirectStandardError "$env:TEMP\probe-err.log"
}
try {
    foreach ($i in 1..60) {
        try { Invoke-WebRequest "http://localhost:$Port/" -UseBasicParsing -TimeoutSec 30 | Out-Null; break }
        catch { if ($_.Exception.Response) { break }; Start-Sleep 1 }
    }
    foreach ($page in 'Redirect.aspx', 'Target.aspx', 'Encodings.aspx', 'ShiftJis.aspx', 'Serialize.aspx') {
        $request = [Net.HttpWebRequest]::Create("http://localhost:$Port/$page")
        $request.AllowAutoRedirect = $false
        try { $response = $request.GetResponse() } catch [Net.WebException] { $response = $_.Exception.Response }
        $buffer = New-Object IO.MemoryStream
        $response.GetResponseStream().CopyTo($buffer)
        $bytes = $buffer.ToArray()
        $text = [Text.Encoding]::UTF8.GetString($bytes)
        # The page's text as bytes: the response's encoding is what it checks.
        if ($page -eq 'ShiftJis.aspx') { $text = "bytes=" + [BitConverter]::ToString($bytes) }
        elseif ([int]$response.StatusCode -ge 500) {
            $text = [regex]::Match($text, '<title>([^<]*)</title>').Groups[1].Value + ' ' +
                ([regex]::Match($text, '(?s)<b>\s*Exception Details:\s*</b>(.{0,200})').Groups[1].Value -replace '<[^>]+>|\s+', ' ')
        }
        "{0} -> {1} {2} ct={3} loc={4}`n   {5}" -f $page, [int]$response.StatusCode, $response.StatusDescription, $response.ContentType,
            $response.Headers['Location'], ($text.Trim() -replace "`n", "`n   ")
        $response.Close()
    }
}
finally {
    if ($Linux) { docker stop $container | Out-Null }
    else { taskkill /PID $process.Id /T /F 2>&1 | Out-Null }  # the tree: the application runs in a worker process
}
