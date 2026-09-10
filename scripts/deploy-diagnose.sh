#!/usr/bin/env bash
# Safe deploy diagnostics for cloud agents. Prints no secret values.
set -euo pipefail

HOST="${DEPLOY_HOST:-root@45.55.236.179}"

echo "==> secret injection"
for name in DEPLOY_SSH_KEY deploy_ssh_key DEPLOY_SSH_KEY_FILE; do
  if [[ -n "${!name:-}" ]]; then
    echo "  $name: SET"
  else
    echo "  $name: unset"
  fi
done
for name in CLOUD_AGENT_INJECTED_SECRET_NAMES CLOUD_AGENT_ALL_SECRET_NAMES; do
  if [[ -n "${!name:-}" ]]; then
    echo "  $name: ${!name}"
  else
    echo "  $name: unset (Cursor may not be injecting secrets into this session)"
  fi
done

echo "==> cursor ssh-agent (git only — will NOT work for droplet deploy)"
if command -v ssh-add >/dev/null 2>&1; then
  ssh-add -L 2>/dev/null || echo "  (no keys in agent)"
else
  echo "  ssh-add not available"
fi
echo "  note: even with the pubkey on the droplet, Cursor's agent refuses to sign"
echo "  for $HOST (agent refused operation). Use DEPLOY_SSH_KEY instead."

echo "==> explicit-key ssh test to $HOST"
DEPLOY_SSH_KEY="${DEPLOY_SSH_KEY:-${deploy_ssh_key:-}}"
if [[ -z "${DEPLOY_SSH_KEY:-}" && -z "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
  echo "  skipped — DEPLOY_SSH_KEY unset"
else
  KEY_FILE=""
  CLEANUP=0
  if [[ -n "${DEPLOY_SSH_KEY_FILE:-}" ]]; then
    KEY_FILE="$DEPLOY_SSH_KEY_FILE"
  elif [[ -f "${DEPLOY_SSH_KEY}" ]]; then
    KEY_FILE="$DEPLOY_SSH_KEY"
  else
    KEY_FILE="$(mktemp)"
    CLEANUP=1
    printf '%s\n' "$DEPLOY_SSH_KEY" > "$KEY_FILE"
    chmod 600 "$KEY_FILE"
  fi
  if SSH_AUTH_SOCK= ssh -o BatchMode=yes -o ConnectTimeout=15 -o StrictHostKeyChecking=no \
      -o IdentitiesOnly=yes -o IdentityAgent=none -i "$KEY_FILE" "$HOST" "echo ok" 2>/dev/null; then
    echo "  explicit key: OK"
  else
    echo "  explicit key: FAILED (wrong key or pubkey not on droplet)"
  fi
  [[ "$CLEANUP" == 1 ]] && rm -f "$KEY_FILE"
fi
