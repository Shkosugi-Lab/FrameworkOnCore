#!/usr/bin/env bash
# BlogEngine.NET converted on Linux from its source, end to end (the converter's build, the converted application run by
# dotnet, its pages asked for): what the unit tests do not reach (the converter's command line, its build on Linux, the
# host's answers to a real application). The packages workflow's Linux job; locally in the .NET SDK's container.
set -euo pipefail
work=${WORK:-/tmp/foc-be}
rm -rf "$work" && mkdir -p "$work"
curl -sSL -o "$work/be.zip" https://github.com/rxtur/BlogEngine.NET/archive/refs/tags/v3.3.8.0.zip
python3 -c "import zipfile,sys; zipfile.ZipFile(sys.argv[1]).extractall(sys.argv[2])" "$work/be.zip" "$work" 2>/dev/null \
  || (apt-get update -qq >/dev/null && apt-get install -y -qq unzip >/dev/null && unzip -q "$work/be.zip" -d "$work")
dotnet build src/FrameworkOnCore.Converter -v q -nologo
source="$work/BlogEngine.NET-3.3.8.0"
dotnet src/FrameworkOnCore.Converter/bin/Debug/net10.0/FrameworkOnCore.Converter.dll \
  "$source/BlogEngine/BlogEngine.NET/BlogEngine.NET.csproj" --root "$source" --out "$work/out" --build-original auto \
  --runtime experiments/wf4c --deploy none | tail -3
grep -E "^## " "$work/out/CONVERSION-REPORT.md" | head -1
grep -q "^## 未解決(変換で解決できなかったもの)(0 件)" "$work/out/CONVERSION-REPORT.md" || { echo "unresolved left"; exit 1; }
site="$work/out/BlogEngine/BlogEngine.NET"
(cd "$site" && exec dotnet bin/BlogEngine.NET.dll --urls http://127.0.0.1:5077 > "$work/app.log" 2>&1) &
app=$!
trap 'kill $app 2>/dev/null || true' EXIT
status() { curl -s -o /dev/null -w "%{http_code}" --max-time 120 "http://127.0.0.1:5077$1"; }
for i in $(seq 1 60); do [ "$(status /)" = "200" ] && break; sleep 2; done
fail=0
check() { local got; got=$(status "$1"); echo "$got $1"; [ "$got" = "$2" ] || { echo "  expected $2"; fail=1; }; }
check / 200
check /Account/login.aspx 200
check /robots.txt 200
check /web.config 404
check /appsettings.json 404
check /appsettings.json/x 404
check /App_Data/settings.xml 404
check "/(S(abcdefghijklmnopqrstuvwx))/robots.txt" 200
curl -s --max-time 60 "http://127.0.0.1:5077/" | grep -q "BlogEngine" || { echo "the page is not BlogEngine's"; fail=1; }
[ $fail = 0 ] || { tail -40 "$work/app.log"; exit 1; }
echo "BlogEngine.NET on Linux: ok"
