# Installs the converted N2 site (n2\site) through its installer, as a person would: the administrator's
# password, login, the database tables (SQLite, App_Data\n2.sqlite.db), a content package; then "/".
#   .\experiments\wf4c\n2-install.ps1 [-Port 5096] [-Package Plain_SampleData.gz] [-Running]
# -Running: the site is already running on the port (a Linux container: run-linux-site.ps1 -Keep); it is not started here.
param([int]$Port = 5096, [string]$Package = 'Plain_SampleData.gz', [string]$Password = 'N2admin!2026', [switch]$Running)
$page = Join-Path $PSScriptRoot '_linux\page.ps1'
if (-not $Running) {
    $site = Join-Path $PSScriptRoot 'n2\site' | Resolve-Path
    Get-NetTCPConnection -LocalPort $Port -State Listen -ErrorAction SilentlyContinue | ForEach-Object { Stop-Process -Id $_.OwningProcess -Force }
    $log = Join-Path $site 'install.log'
    if (Test-Path $log) { Clear-Content $log }
    $supervise = Join-Path $PSScriptRoot 'supervise.ps1'
    Start-Process powershell -ArgumentList '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', "`"$supervise`"", '-Dll', "`"$site\bin\N2.Templates.dll`"", '-Port', $Port, '-Log', "`"$log`"" -WorkingDirectory $site -WindowStyle Hidden | Out-Null
    foreach ($i in 1..60) { if ((Test-Path $log) -and (Select-String $log -Pattern 'Now listening' -Quiet)) { break }; Start-Sleep 1 }
}
$jar = Join-Path $env:TEMP 'page.cookies'
if (Test-Path $jar) { Clear-Content $jar }
$base = "http://localhost:$Port"

'--- 0. the administrator'
& $page -Url "$base/" -TextLength 200
# The installer's first page, then its form (page.ps1 posts the form of the page requested last). "/" is the site when
# the database already has content (the repository's App_Data\n2.sqlite.db has the sample site).
& $page -Url "$base/N2/Installation/Begin/Default.aspx?action=install" -TextLength 100
& $page -Url "$base/N2/Installation/Begin/Default.aspx?action=install" -Submit ctl11 -Fields @{ chkLoginUrl = 'on'; txtPassword = $Password; txtRepeatPassword = $Password } -TextLength 200
Start-Sleep 5   # web.config changed: the application restarts
'--- login'
& $page -Url "$base/N2/Installation/Default.aspx" -TextLength 100
& $page -Url "$base/N2/Login.aspx?ReturnUrl=%2FN2%2FInstallation%2FDefault.aspx" -Submit 'Login1$LoginButton' -Fields @{ 'Login1$UserName' = 'admin'; 'Login1$Password' = $Password } -TextLength 150
'--- 1. tables'
& $page -Url "$base/N2/Installation/Default.aspx" -Submit btnInstall -TextLength 150
'--- 2. content'
& $page -Url "$base/N2/Installation/Default.aspx" -Submit btnInsertExport -Fields @{ rblExports = $Package } -TextLength 600
'--- the site'
& $page -Url "$base/" -TextLength 400
