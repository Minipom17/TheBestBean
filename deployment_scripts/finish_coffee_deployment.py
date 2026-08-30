#!/usr/bin/env python3
"""
Finish Coffee App Deployment - Skip .NET installation, just configure services
"""
import subprocess
import sys
import os
import time
from pathlib import Path

# Server configuration
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"  # Fixed the typo!
SERVER_USER = "root"
SERVER_PATH = "/srv/coffee_app"

def run_command(cmd, description="", timeout=300):
    """Run a command with timeout handling"""
    print(f"🔄 {description}")
    try:
        result = subprocess.run(cmd, shell=True, check=True, capture_output=True, text=True, timeout=timeout)
        print(f"✅ {description} - Success")
        if result.stdout.strip():
            print(f"   Output: {result.stdout.strip()}")
        return result.stdout
    except subprocess.TimeoutExpired:
        print(f"⏰ {description} - Timeout after {timeout}s")
        return None
    except subprocess.CalledProcessError as e:
        print(f"❌ {description} - Failed")
        print(f"   Error: {e.stderr}")
        return None

def run_remote_command(cmd, description="", timeout=300):
    """Run a command on the remote server"""
    ssh_cmd = f'ssh -i "{SSH_KEY}" -o StrictHostKeyChecking=no -o ConnectTimeout=30 -o ServerAliveInterval=60 {SERVER_USER}@{SERVER_IP} "{cmd}"'
    return run_command(ssh_cmd, description, timeout)

def test_ssh_connection():
    """Test SSH connection"""
    print("\n📡 Testing SSH connection...")
    result = run_remote_command("echo 'SSH connection test successful'", "SSH Connection Test", 60)
    return result is not None

def create_systemd_service():
    """Create systemd service file"""
    print("\n⚙️ Creating systemd service...")
    
    service_content = f"""[Unit]
Description=The Best Bean Coffee App
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory={SERVER_PATH}
ExecStart=/usr/bin/dotnet {SERVER_PATH}/RazorPagesMovie.dll
Restart=always
RestartSec=10
SyslogIdentifier=coffee-app
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
"""
    
    # Write service file locally
    with open("coffee-app.service", "w") as f:
        f.write(service_content)
    
    # Deploy service file
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "coffee-app.service" {SERVER_USER}@{SERVER_IP}:/etc/systemd/system/coffee-app.service'
    if not run_command(scp_cmd, "Deploying systemd service file", 60):
        return False
    
    return True

def configure_nginx():
    """Configure Nginx for both apps"""
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
    
    # Write nginx config locally
    with open("coffee-app-nginx.conf", "w") as f:
        f.write(nginx_config)
    
    # Deploy nginx config
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "coffee-app-nginx.conf" {SERVER_USER}@{SERVER_IP}:/etc/nginx/sites-available/coffee-app'
    if not run_command(scp_cmd, "Deploying Nginx configuration", 60):
        return False
    
    return True

def enable_services():
    """Enable and start services"""
    print("\n🔄 Enabling and starting services...")
    
    commands = [
        "systemctl daemon-reload",
        "systemctl enable coffee-app.service",
        "ln -sf /etc/nginx/sites-available/coffee-app /etc/nginx/sites-enabled/",
        "nginx -t",
        "systemctl reload nginx",
        "systemctl start coffee-app.service"
    ]
    
    for cmd in commands:
        if not run_remote_command(cmd, f"Running: {cmd}", 120):
            print(f"❌ Failed to run: {cmd}")
            return False
    
    return True

def check_final_status():
    """Check final service status"""
    print("\n📊 Checking final status...")
    
    commands = [
        "systemctl status coffee-app.service --no-pager",
        "curl -I http://localhost:5000/",
        "netstat -tlnp | grep :5000"
    ]
    
    for cmd in commands:
        run_remote_command(cmd, f"Status check: {cmd}", 60)

def main():
    print("🚀 Finishing Coffee App Deployment")
    print("=" * 50)
    print(f"Server: {SERVER_IP}")
    print(f"Path: {SERVER_PATH}")
    print("=" * 50)
    
    # Step 1: Test SSH connection
    if not test_ssh_connection():
        print("❌ SSH connection failed")
        return
    
    # Step 2: Create systemd service
    if not create_systemd_service():
        print("❌ Systemd service creation failed")
        return
    
    # Step 3: Configure Nginx
    if not configure_nginx():
        print("❌ Nginx configuration failed")
        return
    
    # Step 4: Enable and start services
    if not enable_services():
        print("❌ Service enablement failed")
        return
    
    # Step 5: Check status
    check_final_status()
    
    print("\n🎉 Coffee app deployment complete!")
    print("=" * 50)
    print("🌐 Your apps are now live:")
    print(f"   - Django app: http://{SERVER_IP}/esl/")
    print(f"   - Coffee app: http://{SERVER_IP}/coffee/")
    print("=" * 50)

if __name__ == "__main__":
    main()
