set -e
cp -r /src /w && cd /w
dotnet run -c Release 2>&1 | grep -E '^(OK|THROW)|error' | sed 's/^/linux  | /'