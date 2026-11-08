# Builds libfoccase.so for linux-x64 and linux-arm64 into casefs/out/<rid>, against an old glibc (Dockerfile.build:
# build.sh in the build image), and prints the newest glibc version each needs; out/key.txt is the key of what it was
# built from (key.ps1). Not needed to convert: the converter fetches the build of the sources' key that GitHub Actions
# published (frameworkoncore-casefs.yml); this is for a change to them (a build here is taken while its key matches).
#   .\experiments\wf4c\casefs\build.ps1
$ErrorActionPreference = 'Continue'
docker build -q -t foccase-build -f (Join-Path $PSScriptRoot 'Dockerfile.build') $PSScriptRoot | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'build image failed' }
docker run --rm -v "${PSScriptRoot}:/src" foccase-build bash /src/build.sh 2>&1 | ForEach-Object { "$_" } | Where-Object { $_ -match 'needs glibc|error' }
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
Set-Content (Join-Path $PSScriptRoot 'out/key.txt') (& (Join-Path $PSScriptRoot 'key.ps1')) -NoNewline -Encoding ascii
