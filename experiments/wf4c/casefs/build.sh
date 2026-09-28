#!/bin/bash
# Builds libfoccase.so for linux-x64 and linux-arm64 into out/<rid> (in the build image, Dockerfile.build), and prints
# the newest glibc version each needs.
set -e
cd "$(dirname "$0")"
flags="-shared -fPIC -O2 -Wall -Wextra -Wno-unused-parameter -Wno-nonnull-compare -Wno-format-truncation"
mkdir -p out/linux-x64 out/linux-arm64
gcc $flags -o out/linux-x64/libfoccase.so foccase.c -ldl -lpthread
aarch64-linux-gnu-gcc $flags -o out/linux-arm64/libfoccase.so foccase.c -ldl -lpthread
for rid in linux-x64 linux-arm64; do
    tool=objdump
    [ "$rid" = linux-arm64 ] && tool=aarch64-linux-gnu-objdump
    echo "$rid: needs glibc $($tool -T "out/$rid/libfoccase.so" | grep -o 'GLIBC_[0-9.]*' | sort -uV | tail -1)"
done
