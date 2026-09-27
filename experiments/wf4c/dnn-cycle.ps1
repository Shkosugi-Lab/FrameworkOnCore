# DNN from scratch: an empty database, conversion (the deployed site of the Cake build), the unattended
# install, then "/". -Pack packs the fork first.
param([switch]$Pack)
$repo = Resolve-Path (Join-Path $PSScriptRoot '..\..')
Get-NetTCPConnection -LocalPort 5096 -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -match 'supervise.ps1|DotNetNuke.Website.dll' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
if ($Pack) {
    Push-Location (Join-Path $repo 'experiments\wf4c')
    foreach ($i in 1..2) { try { .\pack-fork.ps1 -Build All *> _linux\pack-fork.log } catch {}; if ((Get-Content _linux\pack-fork.log -Tail 1) -match '^packed') { break } }
    Get-Content _linux\pack-fork.log -Tail 1
    Pop-Location
}
sqlcmd -S .\SQLEXPRESS -E -C -b -Q "IF DB_ID('dnn_w2l') IS NOT NULL BEGIN ALTER DATABASE dnn_w2l SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE dnn_w2l END"
Push-Location $repo
.\experiments\wf4c\convert-corpora.ps1 -Only dnn
$install = & .\experiments\wf4c\dnn-install.ps1
$install[0]
($install -join ' ') -replace '.*(Creating Site.{0,300}).*', '$1'
$site = Join-Path $repo 'experiments\wf4c\dnn\site'
curl.exe -s -L --max-redirs 5 -m 600 --retry 10 --retry-connrefused --retry-delay 3 -o "$site\home.html" -w "home: %{http_code} %{url_effective} %{time_total}s`n" http://localhost:5096/
$html = Get-Content "$site\home.html" -Raw -Encoding UTF8
if ($html -match '<title[^>]*>\s*([^<]*)') { "title: $($Matches[1])" }
Pop-Location
