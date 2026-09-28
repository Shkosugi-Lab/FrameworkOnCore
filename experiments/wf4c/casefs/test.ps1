# Builds libfoccase.so and runs CaseProbe on Linux (the ASP.NET runtime image) without it (Linux's behaviour) and with it
# (Windows' behaviour expected of every check). -Platform linux/arm64 runs it on arm64 (emulated by Docker Desktop).
#   .\experiments\wf4c\casefs\test.ps1 [-Platform linux/amd64|linux/arm64] [-NoBuild]
param([string]$Platform = 'linux/amd64', [switch]$NoBuild)
$ErrorActionPreference = 'Continue'
if (-not $NoBuild) { & (Join-Path $PSScriptRoot 'build.ps1') | Out-Null }
$work = Join-Path $PSScriptRoot 'out\probe'
New-Item -ItemType Directory $work -Force | Out-Null
docker run --rm -v "$(Join-Path $PSScriptRoot 'test\CaseProbe'):/src:ro" -v "${work}:/out" mcr.microsoft.com/dotnet/sdk:10.0 `
    bash -c 'cp -r /src /tmp/p && cd /tmp/p && dotnet publish -c Release -o /out -nologo -v q' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'probe build failed' }
$rid = if ($Platform -eq 'linux/arm64') { 'linux-arm64' } else { 'linux-x64' }
$lib = Join-Path $PSScriptRoot "out\$rid"
"--- $Platform, without libfoccase.so (Linux)"
docker run --rm --platform $Platform -v "${work}:/app:ro" mcr.microsoft.com/dotnet/aspnet:10.0 dotnet /app/CaseProbe.dll expect-linux
$linux = $LASTEXITCODE
"--- $Platform, with libfoccase.so ($rid, as on Windows)"
docker run --rm --platform $Platform -v "${work}:/app:ro" -v "${lib}:/foccase:ro" -e LD_PRELOAD=/foccase/libfoccase.so -e FOC_CASE_ROOTS=/probe/root -e FOC_CASE_LOG=1 `
    mcr.microsoft.com/dotnet/aspnet:10.0 dotnet /app/CaseProbe.dll expect-windows
$windows = $LASTEXITCODE
if ($linux -ne 0 -or $windows -ne 0) { exit 1 }
