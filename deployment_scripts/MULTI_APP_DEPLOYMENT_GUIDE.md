# Multi-App Deployment Guide: Adding Projects to a Single Digital Ocean Droplet

This guide explains how to deploy multiple web applications (like Django + .NET Core) on a single Digital Ocean droplet using Nginx as a reverse proxy.

## Table of Contents
1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Server Architecture](#server-architecture)
4. [Deployment Process](#deployment-process)
5. [Nginx Configuration](#nginx-configuration)
6. [Service Management](#service-management)
7. [Troubleshooting](#troubleshooting)
8. [Best Practices](#best-practices)

## Overview

When you have multiple web applications that need to run on the same server, you can use Nginx as a reverse proxy to route traffic to different applications based on URL paths. This approach is cost-effective and allows you to run multiple apps on a single droplet.

### Example Setup
- **Django App**: Runs on `/` (root) and handles most traffic
- **.NET Core App**: Runs on `/coffee/` prefix
- **Static Files**: Served efficiently by Nginx
- **Database**: Shared or separate as needed

## Prerequisites

### Server Requirements
- Ubuntu 20.04+ or similar Linux distribution
- Root access via SSH
- At least 1GB RAM (2GB+ recommended for multiple apps)
- Sufficient disk space for your applications

### Local Requirements
- SSH key pair for server access
- Python 3.6+ for deployment scripts
- Required runtime environments (Python for Django, .NET for .NET apps)

### Applications
- Django application with Gunicorn
- .NET Core application (or other framework)
- Static files properly organized

## Server Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Digital Ocean Droplet                    │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │                  Nginx (Port 80)                   │   │
│  │                                                     │   │
│  │  /          → Django App (Gunicorn Socket)         │   │
│  │  /coffee/   → .NET Core App (Port 5000)           │   │
│  │  /static/   → Static Files (Direct)               │   │
│  │  /media/    → Media Files (Direct)                │   │
│  └─────────────────────────────────────────────────────┘   │
│                                                             │
│  ┌─────────────────┐  ┌─────────────────────────────────┐  │
│  │   Django App    │  │        .NET Core App           │  │
│  │  (Gunicorn)     │  │      (Kestrel Server)          │  │
│  │  Socket:        │  │      Port: 5000                │  │
│  │  /srv/esl_go/   │  │      Path: /srv/coffee_app/    │  │
│  └─────────────────┘  └─────────────────────────────────┘  │
│                                                             │
│  ┌─────────────────────────────────────────────────────┐   │
│  │              Systemd Services                       │   │
│  │  • esl_go.service (Django)                         │   │
│  │  • coffee_app.service (.NET Core)                  │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

## Deployment Process

### Step 1: Prepare Your Applications

#### Django Application
Ensure your Django app is configured for production:
```python
# settings.py
DEBUG = False
ALLOWED_HOSTS = ['your-server-ip', 'your-domain.com']

# Static files
STATIC_URL = '/static/'
STATIC_ROOT = '/srv/esl_go/app/staticfiles/'

# Media files
MEDIA_URL = '/media/'
MEDIA_ROOT = '/srv/esl_go/app/media/'
```

#### .NET Core Application
Configure your .NET app for production:
```json
// appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=coffee.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Step 2: Server Setup

#### Install Required Runtimes

**For Django (if not already installed):**
```bash
# Install Python and pip
apt-get update
apt-get install -y python3 python3-pip python3-venv

# Install Gunicorn
pip3 install gunicorn
```

**For .NET Core:**
```bash
# Add Microsoft package repository
wget -q https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET Core runtime
apt-get update
apt-get install -y dotnet-runtime-9.0 aspnetcore-runtime-9.0
```

#### Install Nginx
```bash
apt-get install -y nginx
systemctl enable nginx
systemctl start nginx
```

### Step 3: Application Deployment

#### Django Deployment
```bash
# Create application directory
mkdir -p /srv/esl_go
cd /srv/esl_go

# Deploy Django application
# (Copy your Django app files here)
# Configure virtual environment
# Install dependencies
# Run migrations
# Collect static files
```

#### .NET Core Deployment
```bash
# Create application directory
mkdir -p /srv/coffee_app
cd /srv/coffee_app

# Deploy .NET application
# (Copy published .NET app files here)
```

### Step 4: Systemd Service Configuration

#### Django Service (`/etc/systemd/system/esl_go.service`)
```ini
[Unit]
Description=ESL Go Django Application
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory=/srv/esl_go
ExecStart=/srv/esl_go/venv/bin/gunicorn --bind unix:/srv/esl_go/run/gunicorn.sock esl_go.wsgi:application
Restart=always
RestartSec=10
SyslogIdentifier=esl_go

[Install]
WantedBy=multi-user.target
```

#### .NET Core Service (`/etc/systemd/system/coffee_app.service`)
```ini
[Unit]
Description=The Best Bean Coffee App
After=network.target

[Service]
Type=simple
User=root
WorkingDirectory=/srv/coffee_app
ExecStart=/usr/bin/dotnet /srv/coffee_app/RazorPagesMovie.dll --urls http://localhost:5000
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=Data Source=coffee.db

[Install]
WantedBy=multi-user.target
```

### Step 5: Enable and Start Services
```bash
# Enable services
systemctl daemon-reload
systemctl enable esl_go.service
systemctl enable coffee_app.service

# Start services
systemctl start esl_go.service
systemctl start coffee_app.service

# Check status
systemctl status esl_go.service
systemctl status coffee_app.service
```

## Nginx Configuration

### Main Configuration (`/etc/nginx/sites-available/esl_go`)
```nginx
server {
    listen 80;
    server_name your-server-ip;
    client_max_body_size 100M;

    # Coffee app routes - route ALL coffee app paths
    location /coffee/ {
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 86400;
    }

    # Rewrite coffee app routes to add /coffee/ prefix
    location ~ ^/(FarmProfiles|CoffeeBeans|Learn|Newbie|Tours|FarmerSurveys)(/.*)?$ {
        return 301 /coffee/$1$2;
    }

    # Coffee app static files
    location /coffee/css/ {
        proxy_pass http://localhost:5000/css/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    location /coffee/js/ {
        proxy_pass http://localhost:5000/js/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    location /coffee/lib/ {
        proxy_pass http://localhost:5000/lib/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    location /coffee/Media/ {
        proxy_pass http://localhost:5000/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Also handle direct Media requests (without /coffee/ prefix)
    location /Media/ {
        proxy_pass http://localhost:5000/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Django static files
    location /static/ {
        alias /srv/esl_go/app/staticfiles/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Django media files
    location /media/ {
        alias /srv/esl_go/app/media/;
        expires 30d;
    }

    # Django application (everything else)
    location / {
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Enable Configuration
```bash
# Enable the configuration
ln -sf /etc/nginx/sites-available/esl_go /etc/nginx/sites-enabled/

# Test configuration
nginx -t

# Reload nginx
systemctl reload nginx
```

## Service Management

### Useful Commands

#### Check Service Status
```bash
systemctl status esl_go.service
systemctl status coffee_app.service
systemctl status nginx
```

#### View Logs
```bash
# Django app logs
journalctl -u esl_go.service -f

# .NET Core app logs
journalctl -u coffee_app.service -f

# Nginx logs
tail -f /var/log/nginx/access.log
tail -f /var/log/nginx/error.log
```

#### Restart Services
```bash
# Restart individual services
systemctl restart esl_go.service
systemctl restart coffee_app.service
systemctl restart nginx

# Restart all services
systemctl restart esl_go.service coffee_app.service nginx
```

#### Stop Services
```bash
systemctl stop esl_go.service
systemctl stop coffee_app.service
```

## Troubleshooting

### Common Issues

#### 1. Service Won't Start
```bash
# Check service status
systemctl status your-service.service

# Check logs
journalctl -u your-service.service -f

# Check if port is in use
netstat -tulpn | grep :5000
```

#### 2. Nginx 404 Errors
```bash
# Test nginx configuration
nginx -t

# Check which configuration is active
ls -la /etc/nginx/sites-enabled/

# Check nginx error logs
tail -f /var/log/nginx/error.log
```

#### 3. Static Files Not Loading
- Verify file paths in nginx configuration
- Check file permissions
- Ensure files exist in the specified directories

#### 4. Database Connection Issues
- Verify connection strings
- Check database service status
- Ensure proper permissions

### Debugging Steps

1. **Test individual components:**
   ```bash
   # Test Django app directly
   curl -I http://localhost:8000/
   
   # Test .NET app directly
   curl -I http://localhost:5000/
   
   # Test through nginx
   curl -I http://your-server-ip/coffee/
   ```

2. **Check file permissions:**
   ```bash
   ls -la /srv/esl_go/
   ls -la /srv/coffee_app/
   ```

3. **Verify port availability:**
   ```bash
   ss -tulpn | grep :5000
   ss -tulpn | grep :80
   ```

## Best Practices

### Security
- Use HTTPS in production (Let's Encrypt)
- Keep applications updated
- Use proper file permissions
- Implement firewall rules

### Performance
- Use nginx for static file serving
- Implement caching strategies
- Monitor resource usage
- Use CDN for static assets

### Monitoring
- Set up log rotation
- Monitor disk space
- Set up alerts for service failures
- Regular backups

### Maintenance
- Regular security updates
- Monitor application logs
- Backup databases regularly
- Test deployment procedures

## Adding More Applications

To add another application:

1. **Install required runtime** (if not already installed)
2. **Create application directory** (`/srv/your_app/`)
3. **Deploy application files**
4. **Create systemd service** (`/etc/systemd/system/your_app.service`)
5. **Update nginx configuration** with new location blocks
6. **Enable and start service**
7. **Test the deployment**

### Example: Adding a Node.js App
```nginx
# Add to nginx configuration
location /api/ {
    proxy_pass http://localhost:3000/;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
}
```

```ini
# Create /etc/systemd/system/api.service
[Unit]
Description=API Service
After=network.target

[Service]
Type=simple
User=www-data
WorkingDirectory=/srv/api
ExecStart=/usr/bin/node server.js
Restart=always
Environment=NODE_ENV=production
Environment=PORT=3000

[Install]
WantedBy=multi-user.target
```

## Conclusion

This multi-app deployment approach allows you to efficiently run multiple web applications on a single server while maintaining good performance and security. The key is proper nginx configuration and systemd service management.

Remember to:
- Test thoroughly before deploying to production
- Monitor your applications regularly
- Keep your systems updated
- Have backup and recovery procedures in place



