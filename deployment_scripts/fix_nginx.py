#!/usr/bin/env python3
import subprocess

# Server configuration
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"
SERVER_USER = "root"

def run_remote_command(cmd, description=""):
    ssh_cmd = f'ssh -i "{SSH_KEY}" -o StrictHostKeyChecking=no {SERVER_USER}@{SERVER_IP} "{cmd}"'
    print(f"Running: {description}")
    try:
        result = subprocess.run(ssh_cmd, shell=True, check=True, capture_output=True, text=True)
        print(f"Success: {description}")
        return True
    except subprocess.CalledProcessError as e:
        print(f"Failed: {description}")
        print(f"Error: {e.stderr}")
        return False

def main():
    print("Fixing Nginx Configuration for Coffee App")
    print("=" * 50)
    
    # 1. First, let's start the coffee app manually
    print("Starting coffee app manually...")
    if not run_remote_command("cd /srv/coffee_app && nohup dotnet RazorPagesMovie.dll --urls http://localhost:5000 > /dev/null 2>&1 &", "Start coffee app"):
        return
    
    # 2. Wait a moment for the app to start
    print("Waiting for app to start...")
    import time
    time.sleep(5)
    
    # 3. Test if coffee app is running locally
    if not run_remote_command("curl -I http://localhost:5000/", "Test local coffee app"):
        print("Coffee app is not running on port 5000")
        return
    
    # 4. Update Nginx configuration
    print("Updating Nginx configuration...")
    nginx_config = f"""server {{
    listen 80;
    server_name {SERVER_IP};
    
    # Coffee app (new) - MUST be before Django
    location /coffee/ {{
        proxy_pass http://localhost:5000/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }}
    
    # Static files for coffee app
    location /coffee/css/ {{
        alias /srv/coffee_app/wwwroot/css/;
    }}
    
    location /coffee/js/ {{
        alias /srv/coffee_app/wwwroot/js/;
    }}
    
    location /coffee/Media/ {{
        alias /srv/coffee_app/wwwroot/Media/;
    }}
    
    # Django app (existing) - all other requests
    location / {{
        include proxy_params;
        proxy_pass http://unix:/srv/esl_go/run/gunicorn.sock;
    }}
}}
"""
    
    # Write nginx config locally
    with open("nginx_fix.conf", "w") as f:
        f.write(nginx_config)
    
    # Deploy nginx config
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "nginx_fix.conf" {SERVER_USER}@{SERVER_IP}:/etc/nginx/sites-available/coffee-app'
    if not run_remote_command(f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no "nginx_fix.conf" {SERVER_USER}@{SERVER_IP}:/etc/nginx/sites-available/coffee-app', "Deploy Nginx config"):
        return
    
    # 5. Enable the new config and reload
    commands = [
        "ln -sf /etc/nginx/sites-available/coffee-app /etc/nginx/sites-enabled/",
        "nginx -t",
        "systemctl reload nginx"
    ]
    
    for cmd in commands:
        if not run_remote_command(cmd, f"Running: {cmd}"):
            return
    
    # 6. Test the coffee app
    print("Testing coffee app...")
    if not run_remote_command("curl -I http://45.55.236.179/coffee/", "Test coffee app via Nginx"):
        return
    
    print("\n🎉 SUCCESS!")
    print("Your coffee app should now be accessible at:")
    print(f"http://{SERVER_IP}/coffee/")
    print(f"http://{SERVER_IP}/esl/ (Django app)")

if __name__ == "__main__":
    main()



