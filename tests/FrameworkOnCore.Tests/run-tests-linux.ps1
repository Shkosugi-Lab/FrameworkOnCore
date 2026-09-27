# Runs FrameworkOnCore.Tests on Linux (the .NET SDK's container): the compatibility assembly's helpers behave by
# platform (separators, the case of names, URIs, the Windows identity). The projects it builds are copied in, without
# their build output.
#   .\tests\FrameworkOnCore.Tests\run-tests-linux.ps1
param([string]$Image = 'mcr.microsoft.com/dotnet/sdk:10.0')
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$script = @'
set -e
for d in src/FrameworkOnCore.Analyzers experiments/wf4c/shims/FrameworkOnCore.Compat tests/FrameworkOnCore.Tests; do
  mkdir -p /w/$d
  (cd /repo/$d && find . -type f -not -path './bin/*' -not -path './obj/*' -exec cp --parents {} /w/$d/ \;)
done
cd /w
dotnet test tests/FrameworkOnCore.Tests/FrameworkOnCore.Tests.csproj -nologo -v q
'@ -replace "`r", ''
docker run --rm -v "${repo}:/repo:ro" $Image bash -c $script
