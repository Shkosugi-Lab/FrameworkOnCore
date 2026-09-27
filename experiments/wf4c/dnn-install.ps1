# Starts the converted DNN site under the supervisor and runs its unattended install (Install.aspx?mode=install).
param([int]$Port = 5096)
Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -match 'supervise.ps1|DotNetNuke.Website.dll' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
$site = Join-Path $PSScriptRoot 'dnn\site' | Resolve-Path
$log = Join-Path $site 'install.log'
if (Test-Path $log) { Remove-Item -LiteralPath $log }
$supervise = Join-Path $PSScriptRoot 'supervise.ps1' | Resolve-Path
$p = Start-Process powershell -ArgumentList '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', "`"$supervise`"", '-Dll', "`"$site\bin\DotNetNuke.Website.dll`"", '-Port', $Port, '-Log', "`"$log`"" -WorkingDirectory $site -PassThru -WindowStyle Hidden
foreach ($i in 1..60) { if ((Test-Path $log) -and (Select-String $log -Pattern 'Now listening' -Quiet)) { break }; Start-Sleep 1 }
$sw = [Diagnostics.Stopwatch]::StartNew()
$code = curl.exe -s -L --max-redirs 5 -m 3000 --retry 10 --retry-connrefused --retry-delay 3 -o "$site\install.html" -w "%{http_code}" "http://localhost:$Port/Install/Install.aspx?mode=install"
"install: $code in $($sw.Elapsed)"
$html = Get-Content "$site\install.html" -Raw -Encoding UTF8
$text = $html -replace '(?s)<script.*?</script>', '' -replace '<[^>]+>', ' ' -replace '&nbsp;', ' '
$text = $text -replace '\s+', ' '
$text.Substring([Math]::Max(0, $text.Length - 2500))
"supervisor pid $($p.Id) (left running)"
