# Builds libfoccase.so (x64, glibc 2.39: the ASP.NET runtime image's) into casefs/out.
#   .\experiments\wf4c\casefs\build.ps1
$ErrorActionPreference = 'Continue'
docker build -q -t foccase-build -f (Join-Path $PSScriptRoot 'Dockerfile.build') $PSScriptRoot | Out-Null
New-Item -ItemType Directory (Join-Path $PSScriptRoot 'out') -Force | Out-Null
docker run --rm -v "${PSScriptRoot}:/src" -w /src foccase-build gcc -shared -fPIC -O2 -Wall -Wextra -Wno-unused-parameter -Wno-nonnull-compare -Wno-format-truncation -o out/libfoccase.so foccase.c -ldl -lpthread 2>&1 | ForEach-Object { "$_" }
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
"built: $(Join-Path $PSScriptRoot 'out\libfoccase.so')"