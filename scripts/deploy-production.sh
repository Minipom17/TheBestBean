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
chmod 600 "$KEY_FILE"
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
  dotnet publish -c Release -o "$PUBLISH" --nologo --verbosity quiet
)
rm -f "$PUBLISH/coffee.db" "$PUBLISH/coffee.db-shm" "$PUBLISH/coffee.db-wal"
rm -rf "$PUBLISH/wwwroot/Media"

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
tar -xzf /root/deploy_temp.tar.gz --exclude=coffee.db
rm -f /root/deploy_temp.tar.gz
chown -R www-data:www-data "$REMOTE_DIR"
systemctl start coffee-app.service
systemctl is-active coffee-app.service
test -f "$REMOTE_DIR/coffee.db"
echo "deploy ok"
REMOTE

echo "==> https://purplebean.coffee"
