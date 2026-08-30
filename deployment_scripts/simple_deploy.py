#!/usr/bin/env python3
import subprocess
import sys

# Server configuration
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"
SERVER_USER = "root"
SERVER_PATH = "/srv/coffee_app"

def run_command(cmd, description=""):
    print(f"Running: {description}")
    try:
        result = subprocess.run(cmd, shell=True, check=True, capture_output=True, text=True)
        print(f"Success: {description}")
        if result.stdout.strip():
            print(f"Output: {result.stdout.strip()}")
        return result.stdout
    except subprocess.CalledProcessError as e:
        print(f"Failed: {description}")
        print(f"Error: {e.stderr}")
        return None

def run_remote_command(cmd, description=""):
    ssh_cmd = f'ssh -i "{SSH_KEY}" -o StrictHostKeyChecking=no {SERVER_USER}@{SERVER_IP} "{cmd}"'
    return run_command(ssh_cmd, description)

def main():
    print("Coffee App Deployment - Final Setup")
    print("=" * 40)
    
    # Test SSH
    print("Testing SSH connection...")
    if not run_remote_command("echo 'SSH OK'", "SSH Test"):
        print("SSH connection failed!")
        return
    
    # Create systemd service
    print("Creating systemd service...")
    service_content = f"""[Unit]
Description=Coffee App
After=network.target

[Service]
Type=notify
User=www-data
Group=www-data
WorkingDirectory={SERVER_PATH}
ExecStart=/usr/bin/dotnet {SERVER_PATH}/RazorPagesMovie.dll
Restart=always
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
"""
    
    with open("coffee.service", "w") as f:
        f.write(service_content)
    
    # Deploy service file
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "coffee.service" {SERVER_USER}@{SERVER_IP}:/etc/systemd/system/coffee-app.service'
    if not run_command(scp_cmd, "Deploy service file"):
        return
    
    # Configure Nginx
    print("Configuring Nginx...")
    nginx_config = f"""server {{
    listen 80;
    server_name {SERVER_IP};
    
    location /esl/ {{
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }}
    
    location /coffee/ {{
        proxy_pass http://localhost:5000/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }}
    
    location /coffee/css/ {{
        alias {SERVER_PATH}/wwwroot/css/;
    }}
    
    location /coffee/js/ {{
        alias {SERVER_PATH}/wwwroot/js/;
    }}
    
    location /coffee/Media/ {{
        alias {SERVER_PATH}/wwwroot/Media/;
    }}
    
    location / {{
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }}
}}
"""
    
    with open("coffee-nginx.conf", "w") as f:
        f.write(nginx_config)
    
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "coffee-nginx.conf" {SERVER_USER}@{SERVER_IP}:/etc/nginx/sites-available/coffee-app'
    if not run_command(scp_cmd, "Deploy Nginx config"):
        return
    
    # Enable services
    print("Enabling services...")
    commands = [
        "systemctl daemon-reload",
        "systemctl enable coffee-app.service",
        "ln -sf /etc/nginx/sites-available/coffee-app /etc/nginx/sites-enabled/",
        "nginx -t",
        "systemctl reload nginx",
        "systemctl start coffee-app.service"
    ]
    
    for cmd in commands:
        if not run_remote_command(cmd, f"Running: {cmd}"):
            print(f"Failed: {cmd}")
            return
    
    # Check status
    print("Checking status...")
    run_remote_command("systemctl status coffee-app.service --no-pager", "Service status")
    run_remote_command("curl -I http://localhost:5000/", "Test app")
    
    print("\nDeployment complete!")
    print(f"Coffee app: http://{SERVER_IP}/coffee/")
    print(f"Django app: http://{SERVER_IP}/esl/")

if __name__ == "__main__":
    main()
