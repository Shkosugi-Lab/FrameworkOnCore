#!/bin/bash
# Builds ICU data that make .NET's culture data the original server's, for ICU_DATA.
# Runs in the Linux image the application runs in (its ICU version is the one the data are for).
#
#   build-icu-data.sh <culture-profile.json> <out-directory>
#
# Then run the application with ICU_DATA=<out-directory>. Prints what ICU data cannot carry (the
# check at the end compares the runtime's cultures under the new data with the profile).
set -euo pipefail
profile="$1"
out="$2"
here="$(cd "$(dirname "$0")" && pwd)"

if ! command -v genrb >/dev/null; then
    apt-get update -qq >/dev/null
    apt-get install -y -qq icu-devtools >/dev/null
fi
# The runtime's ICU version (e.g. 74.2): the data must be built from the same version's sources.
version="$(icuinfo 2>/dev/null | sed -n 's/.*"version">\([0-9.]*\)<.*/\1/p' | head -1)"
if [ -z "$version" ]; then echo "ICU version not found (icuinfo)" >&2; exit 1; fi
echo "ICU $version"

tool="$(mktemp -d)"
cp -r "$here/CultureIcu/." "$tool"
rm -rf "$tool/bin" "$tool/obj"
dotnet build "$tool" -v q -nologo -o "$tool/out" >/dev/null

mkdir -p "$out"
dotnet "$tool/out/CultureIcu.dll" generate "$profile" "$version" "$out"
echo "--- check (ICU_DATA=$out)"
ICU_DATA="$out" dotnet "$tool/out/CultureIcu.dll" diff "$profile" || true
