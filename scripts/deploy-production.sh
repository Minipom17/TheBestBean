#!/usr/bin/env bash
# Deploy Purple Bean to the DigitalOcean droplet without wiping photos or the live DB.
#
# Uses scripts/deploy_key.txt in the repo (cloud agents read this file).
# Optional override: DEPLOY_SSH_KEY env var or DEPLOY_SSH_KEY_FILE path.
# DEPLOY_HOST defaults to root@45.55.236.179
#
# Never copies a local coffee.db over production.
# Never deletes wwwroot/Media on the server.

set -euo pipefail

HOST="${DEPLOY_HOST:-root@45.55.236.179}"
REMOTE="/srv/coffee_app"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
KEY_FILE=""
CLEANUP_KEY=0

die() { echo "error: $*" >&2; exit 1; }

DEPLOY_SSH_KEY="${DEPLOY_SSH_KEY:-${deploy_ssh_key:-}}"
DEFAULT_KEY_FILE="$ROOT/scripts/deploy_key.txt"

if [[ -z "${DEPLOY_SSH_KEY:-}" && -z "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
  if [[ -f "$DEFAULT_KEY_FILE" ]]; then
    DEPLOY_SSH_KEY_FILE="$DEFAULT_KEY_FILE"
  else
    die "No deploy key at scripts/deploy_key.txt"
  fi
fi

if [[ -n "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
  KEY_FILE="$DEPLOY_SSH_KEY_FILE"
elif [[ -f "${DEPLOY_SSH_KEY}" ]]; then
  KEY_FILE="$DEPLOY_SSH_KEY"
else
  KEY_FILE="$(mktemp)"
  CLEANUP_KEY=1
  printf '%s\n' "$DEPLOY_SSH_KEY" > "$KEY_FILE"
fi
#chmod 600 "$KEY_FILE"
trap '[[ "$CLEANUP_KEY" == 1 ]] && rm -f "$KEY_FILE"' EXIT

SSH_AUTH_SOCK=
SSH=(ssh -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -o IdentityAgent=none -i "$KEY_FILE" "$HOST")
SCP=(env SSH_AUTH_SOCK= scp -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -o IdentityAgent=none -i "$KEY_FILE")

echo "==> checking SSH"
if ! SSH_AUTH_SOCK= "${SSH[@]}" -o BatchMode=yes -o ConnectTimeout=15 "true" 2>/dev/null; then
  die "SSH login failed. Check scripts/deploy_key.txt and droplet authorized_keys."
fi
"${SSH[@]}" "test -d $REMOTE && systemctl is-enabled coffee-app.service >/dev/null"

echo "==> publish (Release, no Media, no local DB)"
PUBLISH="$(mktemp -d)"
trap 'rm -rf "$PUBLISH"; [[ "$CLEANUP_KEY" == 1 ]] && rm -f "$KEY_FILE"' EXIT
(
  cd "$ROOT"
  # Clean so Razor/CSS changes are never skipped by an incremental cache.
  dotnet clean -c Release --nologo --verbosity quiet >/dev/null
  rm -rf "$ROOT/bin/Release" "$ROOT/obj/Release"
  dotnet publish -c Release -o "$PUBLISH" --nologo --verbosity quiet
)
rm -f "$PUBLISH/coffee.db" "$PUBLISH/coffee.db-shm" "$PUBLISH/coffee.db-wal"
rm -rf "$PUBLISH/wwwroot/Media"
# Drop precompressed siblings — stale .gz/.br on the server beat fresh CSS
# (Kestrel serves them with Cache-Control max-age=1y; Safari keeps them longest).
find "$PUBLISH/wwwroot" \( -name '*.gz' -o -name '*.br' \) -type f -delete

TAR="$(mktemp --suffix=.tar.gz)"
trap 'rm -rf "$PUBLISH" "$TAR"; [[ "$CLEANUP_KEY" == 1 ]] && rm -f "$KEY_FILE"' EXIT
tar -czf "$TAR" -C "$PUBLISH" .
echo "==> upload package ($(du -h "$TAR" | cut -f1))"
"${SCP[@]}" "$TAR" "$HOST:/root/deploy_temp.tar.gz"

echo "==> extract on server (keep coffee.db and wwwroot/Media)"
"${SSH[@]}" bash -s <<'REMOTE'
set -euo pipefail
REMOTE_DIR=/srv/coffee_app
systemctl stop coffee-app.service
cp -a "$REMOTE_DIR/coffee.db" "/root/coffee.db.bak-agent-$(date +%Y%m%d%H%M)"
cd "$REMOTE_DIR"
# Remove stale compressed static assets before extract so they cannot win.
find wwwroot \( -name '*.gz' -o -name '*.br' \) -type f -delete 2>/dev/null || true
tar -xzf /root/deploy_temp.tar.gz --exclude=coffee.db
rm -f /root/deploy_temp.tar.gz
chown -R www-data:www-data "$REMOTE_DIR"
systemctl start coffee-app.service
systemctl is-active coffee-app.service
test -f "$REMOTE_DIR/coffee.db"
# Prove the new build landed (utf-16 Razor strings + CSS size).
python3 - <<'PY'
from pathlib import Path
dll = Path("/srv/coffee_app/TheBestBean.dll").read_bytes()
css = Path("/srv/coffee_app/wwwroot/css/site.css")
need = ["2-days", "exp-cta-row", "lab-stock-modal"]
missing = [s for s in need if s.encode("utf-16le") not in dll]
if missing:
    raise SystemExit(f"deploy verify failed: DLL missing {missing}")
if css.stat().st_size < 114000:
    raise SystemExit(f"deploy verify failed: site.css too small ({css.stat().st_size})")
gz = list(Path("/srv/coffee_app/wwwroot/css").glob("site.css.gz"))
if gz:
    raise SystemExit(f"deploy verify failed: stale {gz[0]} still present")
print(f"verify ok dll={len(dll)} css={css.stat().st_size}")
PY
echo "deploy ok"
REMOTE

echo "==> https://purplebean.coffee"
