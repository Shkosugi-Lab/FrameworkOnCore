set -e
cp -r /src /w && cd /w && dotnet build -v q -nologo -o out 2>&1 | grep -E ' error ' | head -5 || true
mkdir -p /tmp/p6 && cd /tmp/p6
curl -sL -o p.zip https://www.nuget.org/api/v2/package/System.Drawing.Common/6.0.0
curl -sL -o w.zip https://www.nuget.org/api/v2/package/Microsoft.Win32.SystemEvents/6.0.0
python3 -c "import zipfile;zipfile.ZipFile('p.zip').extractall('.');zipfile.ZipFile('w.zip').extractall('we')"
(apt-get update -qq && apt-get install -y -qq libgdiplus fonts-dejavu-core >/dev/null 2>&1) && echo installed
cd /w
mkdir -p out/sdc6
cp /tmp/p6/runtimes/unix/lib/net6.0/System.Drawing.Common.dll out/sdc6/
cp /tmp/p6/we/lib/net6.0/Microsoft.Win32.SystemEvents.dll out/sdc6/ 2>/dev/null || true
echo "== 10.0 left out of the application's assemblies; Resolving gives the 6.0 one"
python3 /src/strip.py
dotnet out/DrawingProbe.dll resolve
