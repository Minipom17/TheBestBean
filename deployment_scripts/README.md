# Deployment Scripts

This folder contains various deployment scripts and guides for The Best Bean Coffee App.

## ⚠️ IMPORTANT: PathBase Configuration

The app is deployed at `http://45.55.236.179/coffee/` (subpath, not root).

**Critical:** The `Program.cs` file MUST include PathBase middleware or all navigation links will break!

See `COFFEE_APP_DEPLOYMENT_GUIDE.md` for details.

---

## 🚀 Recommended Deployment Method

Use the PowerShell script in the parent directory:

```powershell
cd C:\Users\alext\source\repos\RazorPagesMovie\RazorPagesMovie
.\deploy_to_production.ps1
```

This is the **fastest and most reliable** method.

---

## 📁 Files in This Folder

### Active Scripts:
- **`COFFEE_APP_DEPLOYMENT_GUIDE.md`** - Comprehensive deployment documentation
- **`quick_deploy_coffee.bat`** - Windows batch deployment script

### Legacy Scripts (Optional):
- `deploy_coffee_app_build.py` - Python deployment with build
- `deploy_coffee_app_robust.py` - Robust Python deployment
- `finish_coffee_deployment.py` - Finalization script
- `simple_deploy.py` - Simple Python deployment
- `test_deployment.py` - Deployment testing
- `fix_nginx.py` - Nginx configuration fixes
- `fix_nginx_config.py` - Nginx config helper

**Note:** The Python scripts have Unicode encoding issues on Windows. Use the PowerShell script instead.

---

## 🗂️ Project Organization

```
RazorPagesMovie/
├── deploy_to_production.ps1    ← USE THIS!
├── deployment_scripts/          ← Legacy scripts & guides
├── deployment_archives/         ← Temporary tar.gz files (auto-cleaned)
├── Pages/                       ← Razor Pages
├── Models/                      ← Data models
├── Services/                    ← Business logic
├── Data/                        ← Database context
├── wwwroot/                     ← Static files
└── coffee.db                    ← SQLite database
```

---

## 🔧 Quick Commands

### Deploy to Production:
```powershell
.\deploy_to_production.ps1
```

### Run Locally:
```powershell
cd C:\Users\alext\source\repos\RazorPagesMovie\RazorPagesMovie
dotnet run
```

### Check Production Status:
```powershell
ssh -i "C:\Users\alext\Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl status coffee-app.service"
```

### View Production Logs:
```powershell
ssh -i "C:\Users\alext\Coffe_Cacoa_Coca.txt" root@45.55.236.179 "journalctl -u coffee-app.service -n 50 --no-pager"
```
