#!/usr/bin/env bash
# Deploy Purple Bean to the DigitalOcean droplet without wiping photos or the live DB.
#
# Cloud agent / Linux:
#   DEPLOY_SSH_KEY must be set (Cursor Runtime Secret: private key contents, or a file path)
#   DEPLOY_HOST defaults to root@45.55.236.179
#   ./scripts/deploy-production.sh
#
# Never prints the key. Never copies a local coffee.db over production.
# Never deletes wwwroot/Media on the server.

set -euo pipefail

HOST="${DEPLOY_HOST:-root@45.55.236.179}"
REMOTE="/srv/coffee_app"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
KEY_FILE=""
CLEANUP_KEY=0

die() { echo "error: $*" >&2; exit 1; }

USE_AGENT=0
if [[ -n "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
  KEY_FILE="$DEPLOY_SSH_KEY_FILE"
elif [[ -n "${DEPLOY_SSH_KEY:-}" && -f "${DEPLOY_SSH_KEY}" ]]; then
  KEY_FILE="$DEPLOY_SSH_KEY"
elif [[ -n "${DEPLOY_SSH_KEY:-}" ]]; then
  KEY_FILE="$(mktemp)"
  CLEANUP_KEY=1
  printf '%s\n' "$DEPLOY_SSH_KEY" > "$KEY_FILE"
elif [[ -n "${SSH_AUTH_SOCK:-}" ]]; then
  USE_AGENT=1
else
  die "No SSH key. Set DEPLOY_SSH_KEY / DEPLOY_SSH_KEY_FILE, or use an ssh-agent (SSH_AUTH_SOCK)."
fi

if [[ "$USE_AGENT" == 1 ]]; then
  SSH=(ssh -o StrictHostKeyChecking=no -o BatchMode=yes "$HOST")
  SCP=(scp -o StrictHostKeyChecking=no -o BatchMode=yes)
else
  chmod 600 "$KEY_FILE"
  trap '[[ "$CLEANUP_KEY" == 1 ]] && rm -f "$KEY_FILE"' EXIT
  SSH=(ssh -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -i "$KEY_FILE" "$HOST")
  SCP=(scp -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -i "$KEY_FILE")
fi

echo "==> checking SSH"
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
