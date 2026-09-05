# TheBestBean (Purple Bean Coffee)

ASP.NET Core 9 Razor Pages app. Local URL: http://localhost:5202

## Cursor Cloud specific instructions

This repo is meant to run in **Cursor Cloud Agents**, not against the Windows PC via Remote Desktop.

- Clone comes from GitHub: `Minipom17/TheBestBean`
- `.cursor/environment.json` installs the .NET 9 SDK, restores, and builds
- App start: `dotnet run --urls http://0.0.0.0:5202`
- SQLite DB is created locally in the cloud VM (`coffee.db`). Do not expect production data.
- Large photos/videos/`wwwroot/Media` are not in git. They live on the production server. Edit CSS, pages, and C# without those files.
- After changes, push a branch and open a PR. Do not deploy to production unless asked.

### Production deploy (only when the user asks)

Cloud agents are **not** on the Windows PC. They cannot use `C:\Users\alext\Coffe_Cacoa_Coca.txt` or `deploy_to_production.ps1`.

When asked to deploy, run:

```bash
chmod +x scripts/deploy-production.sh
./scripts/deploy-production.sh
```

That script needs a Cursor **Runtime Secret** named `DEPLOY_SSH_KEY` (the SSH private key text). Optional env var: `DEPLOY_HOST` (default `root@45.55.236.179`).

Rules:

- Never print, commit, or echo `DEPLOY_SSH_KEY`
- Never copy the cloud VM `coffee.db` over production
- Never `rm -rf` `/srv/coffee_app` or `wwwroot/Media`
- If `DEPLOY_SSH_KEY` is missing, stop and tell the user to add it at https://cursor.com/dashboard/cloud-agents (Secrets)
