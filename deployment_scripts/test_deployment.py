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
    print("Testing coffee app deployment...")
    
    # Check if app files exist
    print("\n1. Checking app files on server...")
    code, stdout, stderr = run_ssh_command("cd /srv/coffee_app && ls -la")
    if code == 0:
        print("✓ App files found")
        print(stdout)
    else:
        print(f"✗ Error checking files: {stderr}")
        return
    
    # Check if coffee.db exists
    print("\n2. Checking SQLite database...")
    code, stdout, stderr = run_ssh_command("cd /srv/coffee_app && ls -la coffee.db")
    if code == 0:
        print("✓ SQLite database found")
        print(stdout)
    else:
        print(f"✗ Database not found: {stderr}")
    
    # Start the app
    print("\n3. Starting the app...")
    code, stdout, stderr = run_ssh_command("cd /srv/coffee_app && nohup dotnet RazorPagesMovie.dll --urls http://localhost:5000 > app.log 2>&1 &")
    if code == 0:
        print("✓ App started")
    else:
        print(f"✗ Error starting app: {stderr}")
    
    # Wait a moment
    print("\n4. Waiting for app to start...")
    time.sleep(5)
    
    # Check if app is running
    print("\n5. Checking if app is running...")
    code, stdout, stderr = run_ssh_command("ps aux | grep dotnet | grep -v grep")
    if code == 0 and stdout.strip():
        print("✓ App is running")
        print(stdout)
    else:
        print("✗ App is not running")
    
    # Test local connection
    print("\n6. Testing local connection...")
    code, stdout, stderr = run_ssh_command("curl -I http://localhost:5000/")
    if code == 0:
        print("✓ Local connection works")
        print(stdout[:200])  # First 200 chars
    else:
        print(f"✗ Local connection failed: {stderr}")
    
    # Check app logs
    print("\n7. Checking app logs...")
    code, stdout, stderr = run_ssh_command("cd /srv/coffee_app && tail -20 app.log")
    if code == 0:
        print("✓ App logs:")
        print(stdout)
    else:
        print(f"✗ Could not read logs: {stderr}")

if __name__ == "__main__":
    main()



