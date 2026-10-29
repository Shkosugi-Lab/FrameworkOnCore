# Runs the runtime's unit tests on Linux, in the .NET SDK's container, the repository mounted read-only at /repo:
#   docker run --rm -m 6g -e FILTER -v "<repo>:/repo:ro" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh
# FILTER: those tests only (dotnet test --filter), each failure written out. The sources are copied in without their
# build output (System.Web built from them here).
set -e
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 rsync >/dev/null 2>&1
for d in FrameworkOnCore.Runtime tests/RuntimeUnitTests translations src/FrameworkOnCore.Analyzers; do
  mkdir -p /w/$d
  rsync -a --exclude bin/ --exclude obj/ --exclude '/nupkg/*' /repo/$d/ /w/$d/
done
# The runtime's NuGet.config names its local feed.
mkdir -p /w/FrameworkOnCore.Runtime/nupkg
cd /w
dotnet build tests/RuntimeUnitTests/RuntimeUnitTests.csproj -nologo -v q 2>&1 | grep -E " error " || true
dotnet test tests/RuntimeUnitTests/RuntimeUnitTests.csproj --no-build -nologo ${FILTER:+--filter "$FILTER"} \
  --logger "console;verbosity=normal" 2>&1 | grep -vE "^\s+at |^\s*$|Passed |Skipped " | head -2000
