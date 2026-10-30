# Runs the runtime's unit tests on Linux, in the .NET SDK's container, the repository mounted read-only at /repo:
#   docker run --rm -m 6g -e FILTER -v "<repo>:/repo:ro" mcr.microsoft.com/dotnet/sdk:10.0 bash /repo/tests/RuntimeUnitTests/run-tests-linux.sh
# FILTER: those tests only (dotnet test --filter), each failure written out. The sources are copied in without their
# build output (System.Web built from them here).
# MATRIX: runs under those cultures and time zones instead, one after another ("culture@zone" separated by spaces:
# "tr-TR@Asia/Tokyo invariant@UTC": tests/Shared/TestEnvironment.cs's FOC_TEST_CULTURE, and TZ), each one's results
# written to /results/linux-RuntimeUnitTests-<culture>-<zone>.trx (-e MATRIX -v "<folder>:/results"): tests/run-matrix.ps1.
set -e
apt-get update -qq >/dev/null 2>&1 && apt-get install -y -qq --no-install-recommends libgdiplus fonts-liberation2 rsync tzdata >/dev/null 2>&1
for d in FrameworkOnCore.Runtime tests/RuntimeUnitTests tests/Shared translations src/FrameworkOnCore.Analyzers; do
  mkdir -p /w/$d
  rsync -a --exclude bin/ --exclude obj/ --exclude '/nupkg/*' /repo/$d/ /w/$d/
done
# The runtime's NuGet.config names its local feed.
mkdir -p /w/FrameworkOnCore.Runtime/nupkg
cd /w
# Built as the CI builds it (its projects side by side). Web.Extensions compiled against .NET's System.Web facade at times
# (CS7069 IHttpHandlerFactory): the projects' removal of the facade matched Windows paths only (fixed: by its reference's
# metadata).
dotnet build tests/RuntimeUnitTests/RuntimeUnitTests.csproj -nologo -v q 2>&1 | grep -E " error " || true
if [ -z "$MATRIX" ]; then
  dotnet test tests/RuntimeUnitTests/RuntimeUnitTests.csproj --no-build -nologo ${FILTER:+--filter "$FILTER"} \
    --logger "console;verbosity=normal" 2>&1 | grep -vE "^\s+at |^\s*$|Passed |Skipped " | head -2000
  exit 0
fi
for entry in $MATRIX; do
  culture=${entry%@*}; zone=${entry#*@}
  echo "== RuntimeUnitTests culture=$culture TZ=${zone:-(none)}"
  # FOC_TEST_TZ as well: the run fails at once if TZ was not taken (an unknown zone).
  FOC_TEST_CULTURE=$culture TZ=$zone FOC_TEST_TZ=$zone dotnet test tests/RuntimeUnitTests/RuntimeUnitTests.csproj --no-build -nologo \
    ${FILTER:+--filter "$FILTER"} --logger "trx;LogFileName=/results/linux-RuntimeUnitTests-$culture-${zone//\//_}.trx" 2>&1 \
    | grep -E "^\s+Failed |Passed!|Failed!| error " | head -300 || true
done
