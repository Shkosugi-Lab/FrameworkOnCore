# Builds libfoccase.so and runs CaseProbe on Linux (the ASP.NET runtime image) without it (Linux's behaviour) and with it
# (Windows' behaviour expected of every check).
#   .\experiments\wf4c\casefs\test.ps1
$ErrorActionPreference = 'Continue'
& (Join-Path $PSScriptRoot 'build.ps1') | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
$work = Join-Path $PSScriptRoot 'out\probe'
New-Item -ItemType Directory $work -Force | Out-Null
docker run --rm -v "$(Join-Path $PSScriptRoot 'test\CaseProbe'):/src:ro" -v "${work}:/out" mcr.microsoft.com/dotnet/sdk:10.0 `
    bash -c 'cp -r /src /tmp/p && cd /tmp/p && dotnet publish -c Release -o /out -nologo -v q' | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'probe build failed' }
$lib = Join-Path $PSScriptRoot 'out'
'--- without libfoccase.so (Linux)'
docker run --rm -v "${work}:/app:ro" mcr.microsoft.com/dotnet/aspnet:10.0 dotnet /app/CaseProbe.dll expect-linux
$linux = $LASTEXITCODE
'--- with libfoccase.so (as on Windows)'
docker run --rm -v "${work}:/app:ro" -v "${lib}:/foccase:ro" -e LD_PRELOAD=/foccase/libfoccase.so -e FOC_CASE_ROOTS=/probe/root -e FOC_CASE_LOG=1 `
    mcr.microsoft.com/dotnet/aspnet:10.0 dotnet /app/CaseProbe.dll expect-windows
$windows = $LASTEXITCODE
if ($linux -ne 0 -or $windows -ne 0) { exit 1 }