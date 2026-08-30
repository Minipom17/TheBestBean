# 🚀 The Best Bean Coffee App - Digital Ocean Deployment Guide

## 📋 **Overview**

This guide will help you deploy your .NET Core Razor Pages coffee application to your existing Digital Ocean server alongside your Django ESL Go app.

**Server Details:**
- **IP:** `45.55.236.179`
- **SSH Key:** `C:/Users/alext/Coffe_Cacoa_Coca.txt`
- **Coffee App Path:** `/srv/coffee_app`
- **Coffee App URL:** `http://45.55.236.179/coffee/`

---

## 🛠️ **Quick Deployment**

### **Option 1: One-Click Deploy (Recommended)**
```bash
# Run this from your project directory
quick_deploy_coffee.bat
```

### **Option 2: Manual Steps**
```bash
# Step 1: Set up server (first time only)
python deploy_coffee_app.py

# Step 2: Build and deploy app
python deploy_coffee_app_build.py
```

---

## 📊 **Server Architecture**

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Your Browser  │───▶│   Nginx (Port 80) │───▶│ Multiple Apps   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │                        │
                              ▼                        ▼
                       Route by URL              Django + .NET Core
                       /esl/ → Django           Coffee App (Port 5000)
                       /coffee/ → .NET Core
```

### **🔧 Components:**
- **Nginx**: Reverse proxy (routes requests by URL path)
- **Django**: Your existing ESL Go app (Unix socket)
- **.NET Core**: Your new coffee app (Port 5000)
- **SQL Server**: Database for coffee app

---

## 🌐 **URL Structure**

| URL | Application | Description |
|-----|-------------|-------------|
| `http://45.55.236.179/` | Django | Your existing ESL Go app |
| `http://45.55.236.179/esl/` | Django | ESL Go app (explicit path) |
| `http://45.55.236.179/coffee/` | .NET Core | The Best Bean Coffee app |

---

## 🛠️ **Server Management Commands**

### **Coffee App Service**
```bash
# Check status
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl status coffee-app.service"

# Start service
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl start coffee-app.service"

# Stop service
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl stop coffee-app.service"

# Restart service
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl restart coffee-app.service"
```

### **View Logs**
```bash
# Coffee app logs
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "journalctl -u coffee-app.service -f"

# Recent logs
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "journalctl -u coffee-app.service --no-pager -n 50"
```

### **Nginx Management**
```bash
# Test configuration
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "nginx -t"

# Reload configuration
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl reload nginx"

# Restart Nginx
ssh -i "C:/Users/alext/Coffe_Cacoa_Coca.txt" root@45.55.236.179 "systemctl restart nginx"
```

---

## 🔄 **Deployment Process**

### **What the scripts do:**

#### **deploy_coffee_app.py** (Server Setup)
1. ✅ Tests SSH connection
2. 🔧 Installs .NET Core 9.0 runtime
3. 📁 Creates application directories
4. ⚙️ Creates systemd service file
5. 🌐 Configures Nginx for both apps
6. 🔄 Enables and starts services

#### **deploy_coffee_app_build.py** (App Deployment)
1. 🔨 Builds .NET app in Release mode
2. 📦 Creates deployment package
3. ⏹️ Stops coffee app service
4. 💾 Backs up existing deployment
5. 📤 Deploys new files to server
6. 🔐 Sets correct permissions
7. ▶️ Starts coffee app service
8. 📊 Verifies deployment

---

## 🔍 **Troubleshooting**

### **Problem: Coffee App Not Loading (404/502 Error)**
```bash
# Check if .NET Core is installed
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "dotnet --version"

# Check if service is running
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl status coffee-app.service"

# Check application logs
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "journalctl -u coffee-app.service --no-pager -n 20"

# Test local access
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "curl -I http://localhost:5000/"
```

### **Problem: Static Files Not Loading**
```bash
# Check static file permissions
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "ls -la /srv/coffee_app/wwwroot/"

# Fix permissions
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "chown -R www-data:www-data /srv/coffee_app/wwwroot/"
```

### **Problem: Database Issues**
```bash
# Check database file
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "ls -la /srv/coffee_app/*.mdf"

# Check application logs for database errors
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "journalctl -u coffee-app.service | grep -i database"
```

### **Problem: Nginx Configuration Issues**
```bash
# Test Nginx configuration
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "nginx -t"

# Check Nginx error logs
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "tail -f /var/log/nginx/error.log"
```

---

## 📊 **Server Monitoring**

### **Check Server Resources**
```bash
# Check disk space
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "df -h"

# Check memory usage
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "free -h"

# Check running processes
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "ps aux | grep -E '(dotnet|gunicorn|nginx)'"
```

### **Check Application Health**
```bash
# Test both applications
curl http://45.55.236.179/esl/
curl http://45.55.236.179/coffee/

# Check service status
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl status coffee-app.service gunicorn-esl_go.service"
```

---

## 🔧 **Configuration Files**

### **Systemd Service** (`/etc/systemd/system/coffee-app.service`)
```ini
[Unit]
Description=The Best Bean Coffee App
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory=/srv/coffee_app
ExecStart=/usr/bin/dotnet /srv/coffee_app/TheBestBean.dll
Restart=always
RestartSec=10
SyslogIdentifier=coffee-app
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

### **Nginx Configuration** (`/etc/nginx/sites-available/coffee-app`)
```nginx
server {
    listen 80;
    server_name 45.55.236.179;
    
    # Django app (existing)
    location /esl/ {
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }
    
    # Coffee app (new)
    location /coffee/ {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
    
    # Static files for coffee app
    location /coffee/css/ {
        alias /srv/coffee_app/wwwroot/css/;
    }
    
    location /coffee/js/ {
        alias /srv/coffee_app/wwwroot/js/;
    }
    
    location /coffee/lib/ {
        alias /srv/coffee_app/wwwroot/lib/;
    }
    
    location /coffee/Media/ {
        alias /srv/coffee_app/wwwroot/Media/;
    }
    
    # Default to Django app
    location / {
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }
}
```

---

## 🚨 **Emergency Procedures**

### **Complete Reset**
```bash
# Stop all services
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl stop coffee-app.service"

# Restart everything
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl restart coffee-app.service && systemctl restart nginx"

# Check status
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl status coffee-app.service"
```

### **Backup Database**
```bash
# Create backup
ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "cp /srv/coffee_app/*.mdf /srv/coffee_app/backup/coffee_db_backup_$(date +%Y%m%d_%H%M%S).mdf"

# Download backup
scp -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179:/srv/coffee_app/backup/coffee_db_backup_*.mdf ./
```

---

## 📞 **Quick Reference**

| Action | Command |
|--------|---------|
| **Deploy App** | `quick_deploy_coffee.bat` |
| **Check Status** | `ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl status coffee-app.service"` |
| **View Logs** | `ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "journalctl -u coffee-app.service -f"` |
| **Restart App** | `ssh -i "C:/Users/alext/Coffe_Cacoa_Caca.txt" root@45.55.236.179 "systemctl restart coffee-app.service"` |
| **Test Apps** | `curl http://45.55.236.179/coffee/` |

---

**🚀 Your coffee app is now ready to serve the world!** ☕



