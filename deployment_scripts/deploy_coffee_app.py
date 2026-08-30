#!/usr/bin/env python3
"""
Deploy "The Best Bean" Coffee App to Digital Ocean Server
This script sets up .NET Core alongside your existing Django app
"""
import subprocess
import sys
import os
from pathlib import Path

# Server configuration
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"
SERVER_USER = "root"
SERVER_PATH = "/srv/coffee_app"
NGINX_SITES_PATH = "/etc/nginx/sites-available"
SYSTEMD_PATH = "/etc/systemd/system"

def run_command(cmd, description=""):
    """Run a command and handle errors"""
    print(f"🔄 {description}")
    try:
        result = subprocess.run(cmd, shell=True, check=True, capture_output=True, text=True)
        print(f"✅ {description} - Success")
        if result.stdout.strip():
            print(f"   Output: {result.stdout.strip()}")
        return result.stdout
    except subprocess.CalledProcessError as e:
        print(f"❌ {description} - Failed")
        print(f"   Error: {e.stderr}")
        return None

def run_remote_command(cmd, description=""):
    """Run a command on the remote server"""
    ssh_cmd = f'ssh -i "{SSH_KEY}" -o StrictHostKeyChecking=no {SERVER_USER}@{SERVER_IP} "{cmd}"'
    return run_command(ssh_cmd, description)

def deploy_file(local_path, remote_path, description=""):
    """Deploy a single file to the server"""
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "{local_path}" {SERVER_USER}@{SERVER_IP}:{remote_path}'
    return run_command(scp_cmd, f"Deploying {description}")

def main():
    print("🚀 The Best Bean Coffee App Deployment")
    print("=" * 50)
    print(f"Server: {SERVER_IP}")
    print(f"Path: {SERVER_PATH}")
    print("=" * 50)
    
    # Step 1: Test SSH connection
    print("\n📡 Testing SSH connection...")
    if not run_remote_command("echo 'SSH connection successful'", "SSH Connection Test"):
        print("❌ SSH connection failed. Please check your SSH key and server IP.")
        return
    
    # Step 2: Install .NET Core
    print("\n🔧 Installing .NET Core...")
    install_commands = [
        "wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb",
        "dpkg -i packages-microsoft-prod.deb",
        "rm packages-microsoft-prod.deb",
        "apt-get update",
        "apt-get install -y apt-transport-https",
        "apt-get update",
        "apt-get install -y dotnet-runtime-9.0",
        "apt-get install -y aspnetcore-runtime-9.0"
    ]
    
    for cmd in install_commands:
        run_remote_command(cmd, f"Installing .NET Core component")
    
    # Step 3: Create application directory
    print("\n📁 Setting up application directory...")
    setup_commands = [
        f"mkdir -p {SERVER_PATH}",
        f"mkdir -p {SERVER_PATH}/logs",
        f"chown -R www-data:www-data {SERVER_PATH}",
        f"chmod -R 755 {SERVER_PATH}"
    ]
    
    for cmd in setup_commands:
        run_remote_command(cmd, "Setting up directories")
    
    # Step 4: Create systemd service file
    print("\n⚙️ Creating systemd service...")
    service_content = f"""[Unit]
Description=The Best Bean Coffee App
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory={SERVER_PATH}
ExecStart=/usr/bin/dotnet {SERVER_PATH}/TheBestBean.dll
Restart=always
RestartSec=10
SyslogIdentifier=coffee-app
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
"""
    
    # Write service file locally then deploy
    with open("coffee-app.service", "w") as f:
        f.write(service_content)
    
    deploy_file("coffee-app.service", f"{SYSTEMD_PATH}/coffee-app.service", "Systemd Service File")
    
    # Step 5: Create Nginx configuration
    print("\n🌐 Configuring Nginx...")
    nginx_config = f"""server {{
    listen 80;
    server_name {SERVER_IP};
    
    # Django app (existing)
    location /esl/ {{
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header Host $host;
        proxy_redirect off;
    }}
    
    # Coffee app (new)
    location /coffee/ {{
        proxy_pass http://localhost:5000/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }}
    
    # Static files for coffee app
    location /coffee/css/ {{
        alias {SERVER_PATH}/wwwroot/css/;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }}
    
    location /coffee/js/ {{
        alias {SERVER_PATH}/wwwroot/js/;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }}
    
    location /coffee/lib/ {{
        alias {SERVER_PATH}/wwwroot/lib/;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }}
    
    location /coffee/Media/ {{
        alias {SERVER_PATH}/wwwroot/Media/;
        expires 1y;
        add_header Cache-Control "public, immutable";
    }}
    
    # Default to Django app for root
    location / {{
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }}
}}
"""
    
    # Write nginx config locally then deploy
    with open("coffee-app-nginx.conf", "w") as f:
        f.write(nginx_config)
    
    deploy_file("coffee-app-nginx.conf", f"{NGINX_SITES_PATH}/coffee-app", "Nginx Configuration")
    
    # Step 6: Enable and reload services
    print("\n🔄 Enabling and starting services...")
    service_commands = [
        "systemctl daemon-reload",
        "systemctl enable coffee-app.service",
        "systemctl start coffee-app.service",
        "ln -sf /etc/nginx/sites-available/coffee-app /etc/nginx/sites-enabled/",
        "nginx -t",
        "systemctl reload nginx"
    ]
    
    for cmd in service_commands:
        run_remote_command(cmd, "Configuring services")
    
    # Step 7: Check service status
    print("\n📊 Checking service status...")
    status_commands = [
        "systemctl status coffee-app.service --no-pager",
        "systemctl status nginx --no-pager"
    ]
    
    for cmd in status_commands:
        run_remote_command(cmd, "Service status check")
    
    print("\n🎉 Server setup complete!")
    print("=" * 50)
    print("📋 Next steps:")
    print("1. Build and deploy your coffee app")
    print("2. Run: python deploy_coffee_app_build.py")
    print("3. Test your apps:")
    print(f"   - Django app: http://{SERVER_IP}/esl/")
    print(f"   - Coffee app: http://{SERVER_IP}/coffee/")
    print("=" * 50)

if __name__ == "__main__":
    main()
