#!/usr/bin/env python3
"""
Build and Deploy The Best Bean Coffee App to Digital Ocean Server
This script builds the .NET app and deploys it to the server
"""
import subprocess
import sys
import os
import shutil
from pathlib import Path

# Server configuration
SERVER_IP = "45.55.236.179"
SSH_KEY = r"C:\Users\alext\Coffe_Cacoa_Coca.txt"
SERVER_USER = "root"
SERVER_PATH = "/srv/coffee_app"

def run_command(cmd, description=""):
    """Run a command and handle errors"""
    print(f"Running: {description}")
    try:
        result = subprocess.run(cmd, shell=True, check=True, capture_output=True, text=True)
        print(f"{description} - Success")
        if result.stdout.strip():
            print(f"   Output: {result.stdout.strip()}")
        return result.stdout
    except subprocess.CalledProcessError as e:
        print(f"{description} - Failed")
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

def deploy_directory(local_path, remote_path, description=""):
    """Deploy a directory to the server"""
    scp_cmd = f'scp -i "{SSH_KEY}" -o StrictHostKeyChecking=no -r "{local_path}" {SERVER_USER}@{SERVER_IP}:{remote_path}'
    return run_command(scp_cmd, f"Deploying {description}")

def main():
    print("Building and Deploying The Best Bean Coffee App")
    print("=" * 60)
    
    # Step 1: Clean and build the application
    print("\nBuilding .NET application...")
    build_commands = [
        "dotnet clean",
        "dotnet restore",
        "dotnet publish -c Release -o ./publish --self-contained false"
    ]
    
    for cmd in build_commands:
        if not run_command(cmd, f"Running: {cmd}"):
            print(f"Build failed at: {cmd}")
            return
    
    # Step 2: Create deployment package
    print("\nCreating deployment package...")
    
    # Create a temporary deployment directory
    deploy_dir = Path("./deploy_temp")
    if deploy_dir.exists():
        shutil.rmtree(deploy_dir)
    deploy_dir.mkdir()
    
    # Copy published files
    publish_dir = Path("./publish")
    if publish_dir.exists():
        shutil.copytree(publish_dir, deploy_dir / "publish")
    
    # Copy wwwroot (static files)
    wwwroot_dir = Path("./wwwroot")
    if wwwroot_dir.exists():
        shutil.copytree(wwwroot_dir, deploy_dir / "wwwroot")
    
    # Copy appsettings.Production.json if it exists
    prod_settings = Path("./appsettings.Production.json")
    if prod_settings.exists():
        shutil.copy2(prod_settings, deploy_dir / "appsettings.Production.json")
    
    # Step 3: Stop the service on server
    print("\nStopping coffee app service...")
    run_remote_command("systemctl stop coffee-app.service", "Stopping coffee app")
    
    # Step 4: Backup existing deployment
    print("\nCreating backup...")
    run_remote_command(f"mkdir -p {SERVER_PATH}/backup", "Creating backup directory")
    run_remote_command(f"cp -r {SERVER_PATH}/publish {SERVER_PATH}/backup/publish_$(date +%Y%m%d_%H%M%S) 2>/dev/null || true", "Backing up existing files")
    
    # Step 5: Deploy new files
    print("\nDeploying application files...")
    
    # Deploy main application
    deploy_directory("./deploy_temp/publish", f"{SERVER_PATH}/", "Application files")
    
    # Deploy static files
    deploy_directory("./deploy_temp/wwwroot", f"{SERVER_PATH}/", "Static files")
    
    # Deploy production settings if exists
    if Path("./deploy_temp/appsettings.Production.json").exists():
        deploy_file("./deploy_temp/appsettings.Production.json", f"{SERVER_PATH}/appsettings.Production.json", "Production settings")
    
    # Step 6: Set permissions
    print("\nSetting file permissions...")
    permission_commands = [
        f"chown -R www-data:www-data {SERVER_PATH}",
        f"chmod -R 755 {SERVER_PATH}",
        f"chmod +x {SERVER_PATH}/TheBestBean.dll"
    ]
    
    for cmd in permission_commands:
        run_remote_command(cmd, "Setting permissions")
    
    # Step 7: Start the service
    print("\nStarting coffee app service...")
    run_remote_command("systemctl start coffee-app.service", "Starting coffee app")
    
    # Step 8: Check service status
    print("\nChecking service status...")
    run_remote_command("systemctl status coffee-app.service --no-pager", "Service status")
    
    # Step 9: Test the application
    print("\nTesting application...")
    run_remote_command("curl -I http://localhost:5000/", "Testing local access")
    
    # Step 10: Cleanup
    print("\nCleaning up...")
    if deploy_dir.exists():
        shutil.rmtree(deploy_dir)
    
    print("\nDeployment complete!")
    print("=" * 60)
    print("Your apps are now live:")
    print(f"   - Django app: http://{SERVER_IP}/esl/")
    print(f"   - Coffee app: http://{SERVER_IP}/coffee/")
    print("=" * 60)
    print("Useful commands:")
    print(f"   - Check logs: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"journalctl -u coffee-app.service -f\"")
    print(f"   - Restart app: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"systemctl restart coffee-app.service\"")
    print(f"   - Check status: ssh -i \"{SSH_KEY}\" {SERVER_USER}@{SERVER_IP} \"systemctl status coffee-app.service\"")
    print("=" * 60)

if __name__ == "__main__":
    main()
