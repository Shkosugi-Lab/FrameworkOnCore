# A converted application behind a reverse proxy (nginx in a container in front of the application's container, as the
# deployment's README has it): what the application sees of the request (samples\RoutingProbe\Where.aspx: the URL, the
# scheme, the client, the redirects' Location) with ASPNETCORE_FORWARDEDHEADERS_ENABLED=true (the proxy's X-Forwarded-For
# and X-Forwarded-Proto taken) and without it. nginx says the request came by HTTPS (X-Forwarded-Proto https), as one that
# ends TLS does. No golden data: IIS behind a proxy is not what the applications ran on; what is checked is that the
# application builds its URLs from the public host, scheme and client when told to.
#   .\experiments\wf4c\proxy-check.ps1      (after sample-parity.ps1 -Name RoutingProbe converted it; Docker)
param([int]$Port = 5096)

$ErrorActionPreference = 'Continue'   # docker's stderr is not an error; exit codes are checked
$converted = Join-Path $PSScriptRoot '_samples\RoutingProbe\converted'
if (-not (Test-Path (Join-Path $converted 'Dockerfile'))) { throw "$converted has no Dockerfile: sample-parity.ps1 -Name RoutingProbe first" }
$work = Join-Path $PSScriptRoot '_samples\RoutingProbe\proxy'
New-Item -ItemType Directory $work -Force | Out-Null
$network = 'w2l-proxy'
$app = 'w2l-proxy-app'
$proxy = 'w2l-proxy-nginx'

docker build -t $app $converted *> (Join-Path $work 'docker-build.log')
if ($LASTEXITCODE -ne 0) { throw "docker build failed (see $work\docker-build.log)" }
cmd /c "docker network create $network" 2>&1 | Out-Null

# nginx: the client's address, the original Host (with its port) and HTTPS as the scheme the client used.
$conf = @"
server {
    listen 80;
    location / {
        proxy_pass http://${app}:8080;
        proxy_set_header Host `$http_host;
        proxy_set_header X-Forwarded-For `$remote_addr;
        proxy_set_header X-Forwarded-Proto https;
    }
}
"@
[IO.File]::WriteAllText((Join-Path $work 'default.conf'), $conf.Replace("`r`n", "`n"))

function Ask([string]$path, [switch]$Location) {
    if ($Location) {
        $headers = curl.exe -s -D - -o NUL --max-time 30 "http://localhost:$Port$path"
        return ($headers | Where-Object { $_ -match '^(HTTP/|Location:)' }) -join ' '
    }
    return (curl.exe -s --max-time 30 "http://localhost:$Port$path") -join ' | '
}

$results = foreach ($forwarded in $false, $true) {
    cmd /c "docker rm -f $app $proxy" 2>&1 | Out-Null
    $envArguments = if ($forwarded) { @('-e', 'ASPNETCORE_FORWARDEDHEADERS_ENABLED=true') } else { @() }
    docker run -d --name $app --network $network @envArguments $app | Out-Null
    docker run -d --name $proxy --network $network -p "${Port}:80" -v "$($work.Replace('\', '/'))/default.conf:/etc/nginx/conf.d/default.conf:ro" nginx:alpine | Out-Null
    foreach ($attempt in 1..90) {
        $status = curl.exe -s -o NUL -w "%{http_code}" --max-time 10 "http://localhost:$Port/Where.aspx"
        if ($status -eq '200') { break }
        Start-Sleep -Seconds 2
    }
    "=== ASPNETCORE_FORWARDEDHEADERS_ENABLED=$forwarded"
    "Where.aspx: " + (Ask '/Where.aspx')
    "a redirect (~/Product.aspx): " + (Ask '/Where.aspx?go=redirect' -Location)
    "a redirect to an absolute URL made from Request.Url: " + (Ask '/Where.aspx?go=absolute' -Location)
    "a page by route: " + ((Ask '/products/5') -replace '.*?(Path=[^|]*?Url=\S+).*', '$1')
}
docker logs $app *> (Join-Path $work 'app.log')
cmd /c "docker rm -f $app $proxy" 2>&1 | Out-Null
cmd /c "docker network rm $network" 2>&1 | Out-Null
cmd /c "docker image rm $app" 2>&1 | Out-Null
$results
