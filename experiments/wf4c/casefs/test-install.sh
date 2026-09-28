#!/bin/bash
# In a plain Linux container (no systemd): the conversion's output at /output installed with deploy/linux/install.sh,
# started as its unit would start it, and "/" requested. The case library's log shows it is loaded.
#   docker run --rm -v <output>:/output:ro ubuntu:22.04 bash /casefs/test-install.sh [case-insensitive: 1|0]
set -e
export DEBIAN_FRONTEND=noninteractive
apt-get update -qq >/dev/null && apt-get install -y -qq curl ca-certificates libicu-dev >/dev/null
cp -r /output /tmp/output
bash /tmp/output/deploy/linux/install.sh --no-start --port 5000 | tail -3
app="$(ls /etc/systemd/system/*.service | head -1 | xargs basename | sed 's/\.service$//')"
echo "FOC_CASE_LOG=1" >> "/etc/$app/environment"
[ "${1:-1}" = 0 ] && echo "FOC_CASE_INSENSITIVE=0" >> "/etc/$app/environment"
user="$(sed -n 's/^User=//p' "/etc/systemd/system/$app.service")"
prefix="$(sed -n 's/^WorkingDirectory=//p' "/etc/systemd/system/$app.service" | sed 's#/site$##')"
# As install.sh tells to start it without systemd: the settings read as root, the application run as its user.
bash -c "set -a; . /etc/$app/service.env; . /etc/$app/environment; exec setpriv --reuid=$user --regid=$user --init-groups bash $prefix/start.sh" > /tmp/app.log 2>&1 &
for i in $(seq 1 120); do
    code="$(curl -s -o /dev/null -w '%{http_code}' -m 60 http://localhost:5000/ || true)"
    [ "$code" != 000 ] && break
    sleep 2
done
echo "glibc $(ldd --version | head -1 | awk '{print $NF}'), $(uname -m): / -> $code"
echo "case library lines: $(grep -c '^foccase:' /tmp/app.log || true)"
grep -m 3 -E '^foccase:|case-sensitive' /tmp/app.log || true
[ "$code" = 000 ] && { echo '--- application log'; tail -25 /tmp/app.log; }
exit 0
