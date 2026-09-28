# Builds libfoccase.so for linux-x64 and linux-arm64 into casefs/out/<rid>, against an old glibc (Dockerfile.build:
# build.sh in the build image), and prints the newest glibc version each needs.
#   .\experiments\wf4c\casefs\build.ps1
$ErrorActionPreference = 'Continue'
docker build -q -t foccase-build -f (Join-Path $PSScriptRoot 'Dockerfile.build') $PSScriptRoot | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'build image failed' }
docker run --rm -v "${PSScriptRoot}:/src" foccase-build bash /src/build.sh 2>&1 | ForEach-Object { "$_" } | Where-Object { $_ -match 'needs glibc|error' }
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
