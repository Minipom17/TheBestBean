#!/usr/bin/env python3
"""
The Best Bean Coffee App - Complete Deployment Script
Deploys a .NET Core Razor Pages app to Digital Ocean alongside existing Django app

This script handles:
- .NET Core runtime installation
- Application building and deployment
- Systemd service configuration
- Nginx reverse proxy setup
- Static file serving
- Service management

Usage: python coffee_app_deploy.py
"""

import subprocess
import sys
import os
import shutil
from pathlib import Path

# =============================================================================
# CONFIGURATION - MODIFY THESE FOR YOUR SETUP
# =============================================================================

# Server details
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"
SERVER_USER = "root"

# Application details
APP_NAME = "coffee_app"
APP_PATH = f"/srv/{APP_NAME}"
APP_DLL = "RazorPagesMovie.dll"
APP_PORT = 5000

# Existing Django app details (for nginx configuration)
DJANGO_SOCKET = "/srv/esl_go/run/gunicorn.sock"
DJANGO_PREFIX = "/esl/"

# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def run_command(cmd, description="", check=True):
    """Run a command and handle errors"""
    print(f"🔄 {description}")
    try:
        result = subprocess.run(cmd, shell=True, check=check, capture_output=True, text=True)
        if result.returncode == 0:
            print(f"✅ {description} - Success")
        else:
            print(f"⚠️ {description} - Warning (Exit code: {result.returncode})")
        
        if result.stdout.strip():
            print(f"   Output: {result.stdout.strip()}")
        if result.stderr.strip() and result.returncode != 0:
            print(f"   Error: {result.stderr.strip()}")
        
        return result.stdout
    except subprocess.CalledProcessError as e:
        print(f"❌ {description} - Failed")
        print(f"   Error: {e.stderr}")
        return None

def run_remote_command(cmd, description="", check=True):
    """Run a command on the remote server"""
    ssh_cmd = f'ssh -i "{SSH_KEY}" -o StrictHostKeyChecking=no {SERVER_USER}@{SERVER_IP} "{cmd}"'
    return run_command(ssh_cmd, description, check)

def deploy_file(local_path, remote_path, description=""):
    """Deploy a single file to the server"""
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "{local_path}" {SERVER_USER}@{SERVER_IP}:{remote_path}'
    return run_command(scp_cmd, f"Deploying {description}")

def deploy_directory(local_path, remote_path, description=""):
    """Deploy a directory to the server"""
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no -r "{local_path}" {SERVER_USER}@{SERVER_IP}:{remote_path}'
    return run_command(scp_cmd, f"Deploying {description}")

# =============================================================================
# DEPLOYMENT STEPS
# =============================================================================

def test_ssh_connection():
    """Test SSH connection to server"""
    print("\n📡 Testing SSH connection...")
    if not run_remote_command("echo 'SSH connection successful'", "SSH Connection Test"):
        print("❌ SSH connection failed. Please check your SSH key and server IP.")
        sys.exit(1)
    return True

def install_dotnet():
    """Install .NET Core runtime on the server"""
    print("\n🔧 Installing .NET Core...")
    
    install_commands = [
        # Add Microsoft package repository
        "wget -q https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb",
        "dpkg -i packages-microsoft-prod.deb",
        "rm packages-microsoft-prod.deb",
        
        # Update package list
        "apt-get update",
        
        # Install .NET Core runtime
        "apt-get install -y dotnet-runtime-9.0 aspnetcore-runtime-9.0",
    ]
    
    for cmd in install_commands:
        if not run_remote_command(cmd, f"Installing .NET Core component", check=False):
            print(f"⚠️ Command failed: {cmd}")
    
    # Verify installation
    run_remote_command("dotnet --version", "Verifying .NET installation")

def setup_application_directory():
    """Create and configure application directory"""
    print("\n📁 Setting up application directory...")
    
    setup_commands = [
        f"mkdir -p {APP_PATH}",
        f"mkdir -p {APP_PATH}/backup",
        f"chown -R www-data:www-data {APP_PATH}",
        f"chmod -R 755 {APP_PATH}"
    ]
    
    for cmd in setup_commands:
        run_remote_command(cmd, "Setting up directories")

def build_application():
    """Build the .NET application locally"""
    print("\n🔨 Building .NET application...")
    
    build_commands = [
        "dotnet clean",
        "dotnet restore",
        "dotnet publish -c Release -o ./publish --self-contained false"
    ]
    
    for cmd in build_commands:
        if not run_command(cmd, f"Running: {cmd}"):
            print(f"❌ Build failed at: {cmd}")
            sys.exit(1)

def deploy_application():
    """Deploy application files to server"""
    print("\n📤 Deploying application...")
    
    # Create deployment package
    deploy_dir = Path("./deploy_temp")
    if deploy_dir.exists():
        shutil.rmtree(deploy_dir)
    deploy_dir.mkdir()
    
    # Copy published files
    publish_dir = Path("./publish")
    if publish_dir.exists():
        shutil.copytree(publish_dir, deploy_dir / "publish")
    
    # Copy static files
    wwwroot_dir = Path("./wwwroot")
    if wwwroot_dir.exists():
        shutil.copytree(wwwroot_dir, deploy_dir / "wwwroot")
    
    # Copy production settings if exists
    prod_settings = Path("./appsettings.Production.json")
    if prod_settings.exists():
        shutil.copy2(prod_settings, deploy_dir / "appsettings.Production.json")
    
    # Stop existing service
    run_remote_command(f"systemctl stop {APP_NAME}.service", "Stopping existing service", check=False)
    
    # Create backup
    run_remote_command(f"cp -r {APP_PATH}/publish {APP_PATH}/backup/publish_$(date +%Y%m%d_%H%M%S) 2>/dev/null || true", "Creating backup", check=False)
    
    # Deploy files
    deploy_directory("./deploy_temp/publish", f"{APP_PATH}/", "Application files")
    deploy_directory("./deploy_temp/wwwroot", f"{APP_PATH}/", "Static files")
    
    if Path("./deploy_temp/appsettings.Production.json").exists():
        deploy_file("./deploy_temp/appsettings.Production.json", f"{APP_PATH}/appsettings.Production.json", "Production settings")
    
    # Set permissions
    permission_commands = [
        f"chown -R www-data:www-data {APP_PATH}",
        f"chmod -R 755 {APP_PATH}",
        f"chmod +x {APP_PATH}/{APP_DLL}"
    ]
    
    for cmd in permission_commands:
        run_remote_command(cmd, "Setting permissions")
    
    # Cleanup
    if deploy_dir.exists():
        shutil.rmtree(deploy_dir)

def create_systemd_service():
    """Create systemd service file"""
    print("\n⚙️ Creating systemd service...")
    
    service_content = f"""[Unit]
Description=The Best Bean Coffee App
After=network.target

[Service]
Type=simple
User=root
WorkingDirectory={APP_PATH}
ExecStart=/usr/bin/dotnet {APP_PATH}/{APP_DLL} --urls http://localhost:{APP_PORT}
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=Data Source=coffee.db

[Install]
WantedBy=multi-user.target
"""
    
    with open("coffee-app.service", "w") as f:
        f.write(service_content)
    
    deploy_file("coffee-app.service", f"/etc/systemd/system/{APP_NAME}.service", "Systemd Service File")
    
    # Enable and start service
    service_commands = [
        "systemctl daemon-reload",
        f"systemctl enable {APP_NAME}.service",
        f"systemctl start {APP_NAME}.service"
    ]
    
    for cmd in service_commands:
        run_remote_command(cmd, "Configuring service")

def configure_nginx():
    """Configure nginx reverse proxy"""
    print("\n🌐 Configuring Nginx...")
    
    nginx_config = f"""server {{
    listen 80;
    server_name {SERVER_IP};
    client_max_body_size 100M;

    # Coffee app routes - route ALL coffee app paths
    location /coffee/ {{
        proxy_pass http://localhost:{APP_PORT}/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 86400;
    }}

    # Rewrite coffee app routes to add /coffee/ prefix
    location ~ ^/(FarmProfiles|CoffeeBeans|Learn|Newbie|Tours|FarmerSurveys)(/.*)?$ {{
        return 301 /coffee/$1$2;
    }}

    # Coffee app static files
    location /coffee/css/ {{
        proxy_pass http://localhost:{APP_PORT}/css/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/js/ {{
        proxy_pass http://localhost:{APP_PORT}/js/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/lib/ {{
        proxy_pass http://localhost:{APP_PORT}/lib/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/Media/ {{
        proxy_pass http://localhost:{APP_PORT}/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Also handle direct Media requests (without /coffee/ prefix)
    location /Media/ {{
        proxy_pass http://localhost:{APP_PORT}/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Django static files
    location /static/ {{
        alias /srv/esl_go/app/staticfiles/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Django media files
    location /media/ {{
        alias /srv/esl_go/app/media/;
        expires 30d;
    }}

    # Django application (everything else)
    location / {{
        proxy_pass http://unix:{DJANGO_SOCKET};
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }}
}}
"""
    
    with open("coffee-app-nginx.conf", "w") as f:
        f.write(nginx_config)
    
    deploy_file("coffee-app-nginx.conf", f"/etc/nginx/sites-available/{APP_NAME}", "Nginx Configuration")
    
    # Update the existing esl_go config (since that's what's being used)
    update_esl_go_config()

def update_esl_go_config():
    """Update the existing esl_go nginx configuration"""
    print("\n🔄 Updating existing nginx configuration...")
    
    # Read current config and update it
    config_update = f"""
# Coffee app routes
location /coffee/ {{
    proxy_pass http://localhost:{APP_PORT}/;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection keep-alive;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_cache_bypass $http_upgrade;
    proxy_read_timeout 86400;
}}

# Rewrite coffee app routes to add /coffee/ prefix
location ~ ^/(FarmProfiles|CoffeeBeans|Learn|Newbie|Tours|FarmerSurveys)(/.*)?$ {{
    return 301 /coffee/$1$2;
}}

# Coffee app static files
location /coffee/css/ {{
    proxy_pass http://localhost:{APP_PORT}/css/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}}

location /coffee/js/ {{
    proxy_pass http://localhost:{APP_PORT}/js/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}}

location /coffee/lib/ {{
    proxy_pass http://localhost:{APP_PORT}/lib/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}}

location /coffee/Media/ {{
    proxy_pass http://localhost:{APP_PORT}/Media/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}}

# Also handle direct Media requests (without /coffee/ prefix)
location /Media/ {{
    proxy_pass http://localhost:{APP_PORT}/Media/;
    expires 30d;
    add_header Cache-Control "public, immutable";
}}
"""
    
    # Update the esl_go config file directly on server
    update_cmd = f"""cat > /tmp/esl_go_update.conf << 'EOF'
server {{
    listen 80;
    server_name {SERVER_IP};
    client_max_body_size 100M;

    # Coffee app routes
    location /coffee/ {{
        proxy_pass http://localhost:{APP_PORT}/;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 86400;
    }}

    # Rewrite coffee app routes to add /coffee/ prefix
    location ~ ^/(FarmProfiles|CoffeeBeans|Learn|Newbie|Tours|FarmerSurveys)(/.*)?$ {{
        return 301 /coffee/$1$2;
    }}

    # Coffee app static files
    location /coffee/css/ {{
        proxy_pass http://localhost:{APP_PORT}/css/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/js/ {{
        proxy_pass http://localhost:{APP_PORT}/js/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/lib/ {{
        proxy_pass http://localhost:{APP_PORT}/lib/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    location /coffee/Media/ {{
        proxy_pass http://localhost:{APP_PORT}/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Also handle direct Media requests (without /coffee/ prefix)
    location /Media/ {{
        proxy_pass http://localhost:{APP_PORT}/Media/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Static files
    location /static/ {{
        alias /srv/esl_go/app/staticfiles/;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }}

    # Media files
    location /media/ {{
        alias /srv/esl_go/app/media/;
        expires 30d;
    }}

    # Django application (everything else)
    location / {{
        proxy_pass http://unix:{DJANGO_SOCKET};
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }}
}}
EOF
mv /tmp/esl_go_update.conf /etc/nginx/sites-available/esl_go"""
    
    run_remote_command(update_cmd, "Updating nginx configuration")
    
    # Test and reload nginx
    nginx_commands = [
        "nginx -t",
        "systemctl reload nginx"
    ]
    
    for cmd in nginx_commands:
        run_remote_command(cmd, "Configuring nginx")

def verify_deployment():
    """Verify the deployment is working"""
    print("\n🧪 Verifying deployment...")
    
    verification_commands = [
        f"systemctl status {APP_NAME}.service --no-pager",
        f"curl -I http://localhost:{APP_PORT}/",
        f"curl -I http://localhost/coffee/",
        f"curl -I http://localhost/FarmProfiles"
    ]
    
    for cmd in verification_commands:
        run_remote_command(cmd, "Verification test", check=False)

# =============================================================================
# MAIN DEPLOYMENT FUNCTION
# =============================================================================

def main():
    """Main deployment function"""
    print("🚀 The Best Bean Coffee App - Complete Deployment")
    print("=" * 60)
    print(f"Server: {SERVER_IP}")
    print(f"App Path: {APP_PATH}")
    print(f"App Port: {APP_PORT}")
    print("=" * 60)
    
    try:
        # Step 1: Test connection
        test_ssh_connection()
        
        # Step 2: Install .NET Core
        install_dotnet()
        
        # Step 3: Setup directories
        setup_application_directory()
        
        # Step 4: Build application
        build_application()
        
        # Step 5: Deploy application
        deploy_application()
        
        # Step 6: Create systemd service
        create_systemd_service()
        
        # Step 7: Configure nginx
        configure_nginx()
        
        # Step 8: Verify deployment
        verify_deployment()
        
        print("\n🎉 Deployment completed successfully!")
        print("=" * 60)
        print("🌐 Your applications are now live:")
        print(f"   - Coffee app: http://{SERVER_IP}/coffee/")
        print(f"   - Django app: http://{SERVER_IP}/")
        print("=" * 60)
        print("📋 Useful commands:")
        print(f"   - Check logs: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"journalctl -u {APP_NAME}.service -f\"")
        print(f"   - Restart app: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"systemctl restart {APP_NAME}.service\"")
        print(f"   - Check status: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"systemctl status {APP_NAME}.service\"")
        print("=" * 60)
        
    except KeyboardInterrupt:
        print("\n⚠️ Deployment cancelled by user")
        sys.exit(1)
    except Exception as e:
        print(f"\n❌ Deployment failed: {e}")
        sys.exit(1)
    finally:
        # Cleanup local files
        cleanup_files = ["coffee-app.service", "coffee-app-nginx.conf"]
        for file in cleanup_files:
            if os.path.exists(file):
                os.remove(file)
                print(f"🧹 Cleaned up: {file}")

if __name__ == "__main__":
    main()



