#!/usr/bin/env python3

import subprocess
import time

def run_ssh_command(command):
    """Run SSH command with proper key"""
    ssh_cmd = [
        'ssh', 
        '-i', 'C:\\Users\\alext\\Coffe_Cacoa_Coca.txt',
        '-o', 'StrictHostKeyChecking=no',
        'root@45.55.236.179',
        command
    ]
    
    try:
        result = subprocess.run(ssh_cmd, capture_output=True, text=True, timeout=30)
        return result.returncode, result.stdout, result.stderr
    except subprocess.TimeoutExpired:
        return -1, "", "Command timed out"
    except Exception as e:
        return -1, "", str(e)

def main():
    print("Configuring Nginx for coffee app...")
    
    # Backup current nginx config
    print("\n1. Backing up current Nginx config...")
    code, stdout, stderr = run_ssh_command("cp /etc/nginx/sites-available/default /etc/nginx/sites-available/default.backup")
    if code == 0:
        print("✓ Backup created")
    else:
        print(f"✗ Backup failed: {stderr}")
    
    # Create new nginx config
    print("\n2. Creating new Nginx configuration...")
    nginx_config = '''
server {
    listen 80 default_server;
    listen [::]:80 default_server;

    root /var/www/html;
    index index.html index.htm index.nginx-debian.html;

    server_name _;

    # Coffee app routes
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

    # Django app routes (everything else)
    location / {
        proxy_pass http://localhost:8000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # Static files for Django
    location /static/ {
        alias /home/ubuntu/esl_go/staticfiles/;
    }

    location /media/ {
        alias /home/ubuntu/esl_go/media/;
    }
}
'''
    
    # Write the config to server
    code, stdout, stderr = run_ssh_command(f"cat > /etc/nginx/sites-available/default << 'EOF'\n{nginx_config}\nEOF")
    if code == 0:
        print("✓ Nginx config created")
    else:
        print(f"✗ Config creation failed: {stderr}")
        return
    
    # Test nginx config
    print("\n3. Testing Nginx configuration...")
    code, stdout, stderr = run_ssh_command("nginx -t")
    if code == 0:
        print("✓ Nginx config is valid")
        print(stdout)
    else:
        print(f"✗ Nginx config error: {stderr}")
        return
    
    # Reload nginx
    print("\n4. Reloading Nginx...")
    code, stdout, stderr = run_ssh_command("systemctl reload nginx")
    if code == 0:
        print("✓ Nginx reloaded successfully")
    else:
        print(f"✗ Nginx reload failed: {stderr}")
        return
    
    # Check if coffee app is running
    print("\n5. Checking if coffee app is running...")
    code, stdout, stderr = run_ssh_command("ps aux | grep dotnet | grep -v grep")
    if code == 0 and stdout.strip():
        print("✓ Coffee app is running")
    else:
        print("✗ Coffee app is not running, starting it...")
        code, stdout, stderr = run_ssh_command("cd /srv/coffee_app && nohup dotnet RazorPagesMovie.dll --urls http://localhost:5000 > app.log 2>&1 &")
        if code == 0:
            print("✓ Coffee app started")
            time.sleep(3)
        else:
            print(f"✗ Failed to start coffee app: {stderr}")
    
    print("\n6. Testing the coffee app...")
    code, stdout, stderr = run_ssh_command("curl -I http://localhost:5000/")
    if code == 0:
        print("✓ Coffee app responds locally")
    else:
        print(f"✗ Coffee app not responding: {stderr}")
    
    print("\n🎉 Configuration complete!")
    print("Try accessing: http://45.55.236.179/coffee/")

if __name__ == "__main__":
    main()



