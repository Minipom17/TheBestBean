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
