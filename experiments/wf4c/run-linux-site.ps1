# Runs a converted site (FrameworkOnCore's <out>\site) in a Linux container as it is deployed: the site
# folder copied into the ASP.NET runtime image (mcr.microsoft.com/dotnet/aspnet:10.0), no build, the web
# assembly started from bin. SQL Server runs in a container too (w2l-sql, kept between runs): the site's
# connection strings to a local instance (.\SQLEXPRESS, (local), localhost) are pointed at it, database
# -Database. Then the paths are requested in order, and each one's status and title printed.
#
#   .\experiments\wf4c\run-linux-site.ps1 -Site dnn\site -Dll DotNetNuke.Website -Database dnn_linux -Paths '/Install/Install.aspx?mode=install', '/'
param(
    # The site folder, relative to experiments\wf4c.
    [Parameter(Mandatory = $true)][string]$Site,
    # The web project's assembly (bin\<Dll>.dll).
    [Parameter(Mandatory = $true)][string]$Dll,
    [string]$Database,
    [string[]]$Paths = @('/'),
    [int]$Port = 5098,
    [string]$Image = 'mcr.microsoft.com/dotnet/aspnet:10.0',
    [string]$Culture = 'en-US',
    # The error details to remote requests too (customErrors Off: the host is not "local" to the container).
    [switch]$ShowErrors,
    [switch]$Keep
)

$ErrorActionPreference = 'Continue'
$siteDirectory = (Resolve-Path (Join-Path $PSScriptRoot $Site)).Path
$name = ($Site -split '[\\/]')[0]
$container = "w2l-site-$($name.ToLowerInvariant() -replace '[^a-z0-9]', '-')"
$logDir = Join-Path $PSScriptRoot "_linux\$name-site"
New-Item -ItemType Directory $logDir -Force | Out-Null
$overlay = Join-Path $logDir 'overlay'
if (Test-Path $overlay) { Remove-Item $overlay -Recurse -Force }
New-Item -ItemType Directory $overlay -Force | Out-Null

# SQL Server (as run-linux.ps1 runs it).
$sqlPassword = 'W2l_local_Passw0rd'   # a throwaway local container
docker network create w2l 2>$null | Out-Null
if ((docker inspect -f '{{.State.Running}}' w2l-sql 2>$null) -ne 'true') {
    docker rm -f w2l-sql 2>$null | Out-Null
    docker run -d --name w2l-sql --network w2l -e ACCEPT_EULA=Y -e "MSSQL_SA_PASSWORD=$sqlPassword" mcr.microsoft.com/mssql/server:2022-latest | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'could not start SQL Server' }
}
foreach ($attempt in 1..60) {
    docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -Q 'SELECT 1' 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) { break }
    Start-Sleep -Seconds 2
}
if ($Database) {
    docker exec w2l-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $sqlPassword -C -b -Q "IF DB_ID('$Database') IS NOT NULL BEGIN ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$Database] END; CREATE DATABASE [$Database]" | Out-Null
}

# The site's .config files (web.config, and those it includes: mojoPortal's user.config): every connection
# string to a local SQL Server, anywhere in them (connectionStrings, appSettings).
$new = "Data Source=w2l-sql;Initial Catalog=$Database;User ID=sa;Password=$sqlPassword;TrustServerCertificate=True"
foreach ($configFile in Get-ChildItem $siteDirectory -Filter '*.config' -File) {
    $text = [IO.File]::ReadAllText($configFile.FullName)
    $changed = [regex]::Replace($text, '(?<=(connectionString|value)=")[^"]*(Data Source|Server)=(\.|\(local\)|localhost|\(LocalDB\))[^"]*(?=")', $new, 'IgnoreCase')
    if ($ShowErrors -and $configFile.Name -eq 'web.config') { $changed = [regex]::Replace($changed, '<customErrors\s+mode="[^"]*"', '<customErrors mode="Off"') }
    if ($changed -ne $text) { [IO.File]::WriteAllText((Join-Path $overlay $configFile.Name), $changed, (New-Object Text.UTF8Encoding $false)) }
}

$script = @"
set -e
mkdir -p /app
cp -r /site/. /app
cp -r /overlay/. /app
cd /app
exec dotnet bin/$Dll.dll --urls http://0.0.0.0:$Port
"@ -replace "`r", ''

docker rm -f $container 2>$null | Out-Null
docker run -d --name $container --network w2l -e "LANG=$($Culture.Replace('-', '_')).UTF-8" -p "${Port}:${Port}" -v "${siteDirectory}:/site:ro" -v "${overlay}:/overlay:ro" $Image bash -c $script | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'docker run failed' }
try {
    $ready = $false
    foreach ($attempt in 1..120) {
        if ((docker inspect -f '{{.State.Running}}' $container) -ne 'true') { break }
        if ((docker logs $container 2>&1 | Select-String 'Now listening' -Quiet)) { $ready = $true; break }
        Start-Sleep -Seconds 2
    }
    if (-not $ready) { docker logs $container 2>&1 | Select-Object -Last 40; throw 'the site did not start in the container (log above)' }
    $i = 0
    foreach ($path in $Paths) {
        $i++
        $file = Join-Path $logDir "response-$i.html"
        $out = curl.exe -s -L --max-redirs 5 -m 1800 --retry 10 --retry-connrefused --retry-delay 3 -o $file -w "%{http_code} %{url_effective} %{time_total}s" "http://localhost:$Port$path"
        $html = if (Test-Path $file) { Get-Content $file -Raw -Encoding UTF8 } else { '' }
        $title = if ($html -match '<title[^>]*>\s*([^<]*?)\s*</title>') { $Matches[1] } else { '' }
        "=== $path -> $out title='$title'"
    }
}
finally {
    docker logs $container *> (Join-Path $logDir 'container.log')
    if (-not $Keep) { docker rm -f $container 2>$null | Out-Null }
}
