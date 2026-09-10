#!/usr/bin/env bash
# Deploy Purple Bean to the DigitalOcean droplet without wiping photos or the live DB.
#
# Cloud agent / Linux:
#   DEPLOY_SSH_KEY must be set (Cursor secret: full private key text)
#   DEPLOY_HOST defaults to root@45.55.236.179
#   ./scripts/deploy-production.sh
#
# Cursor's ssh-agent only signs for git hosts — it will NOT work for the droplet
# ("agent refused operation"). This script always uses an explicit key file and
# disables the agent (SSH_AUTH_SOCK=, IdentityAgent=none).
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

# Cursor sometimes injects secrets under different names or not at all (mobile/web agents).
DEPLOY_SSH_KEY="${DEPLOY_SSH_KEY:-${deploy_ssh_key:-}}"

if [[ -z "${DEPLOY_SSH_KEY:-}" && -z "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
  die "DEPLOY_SSH_KEY is unset. Cursor's ssh-agent cannot sign for the droplet (agent refused operation). Add a Personal secret named DEPLOY_SSH_KEY with the deploy private key, start a NEW cloud agent, then run ./scripts/deploy-diagnose.sh to confirm it shows SET."
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

# Never use Cursor's ssh-agent — it refuses to sign for arbitrary hosts.
SSH_AUTH_SOCK=
SSH=(ssh -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -o IdentityAgent=none -i "$KEY_FILE" "$HOST")
SCP=(env SSH_AUTH_SOCK= scp -o StrictHostKeyChecking=no -o IdentitiesOnly=yes -o IdentityAgent=none -i "$KEY_FILE")

echo "==> checking SSH (explicit key, agent disabled)"
if ! SSH_AUTH_SOCK= "${SSH[@]}" -o BatchMode=yes -o ConnectTimeout=15 "true" 2>/dev/null; then
  die "SSH login failed with DEPLOY_SSH_KEY. Check the secret value (full private key) and that its public key is in the droplet authorized_keys. Run ./scripts/deploy-diagnose.sh"
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
