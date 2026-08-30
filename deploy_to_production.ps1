# Llama Coffee App - Production Deployment Script
# Usage: .\deploy_to_production.ps1

Write-Host "================================" -ForegroundColor Cyan
Write-Host "Llama Coffee - Production Deploy" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$SSH_KEY = "C:\Users\alext\Coffe_Cacoa_Coca.txt"
$SERVER = "root@45.55.236.179"
$DEPLOY_PATH = "/srv/coffee_app"
$PROJECT_PATH = "C:\Users\alext\source\repos\TheBestBean\TheBestBean"

# Step 1: Kill local process
Write-Host "[1/7] Stopping local app..." -ForegroundColor Yellow
taskkill /F /IM TheBestBean.exe 2>$null
if ($LASTEXITCODE -eq 0) {
    Write-Host "  Local app stopped" -ForegroundColor Green
} else {
    Write-Host "  No local app running" -ForegroundColor Gray
}

# Step 2: Build
Write-Host "[2/7] Building application..." -ForegroundColor Yellow
Set-Location $PROJECT_PATH
dotnet publish -c Release -o publish --force --nologo --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "  Build successful" -ForegroundColor Green
} else {
    Write-Host "  Build failed!" -ForegroundColor Red
    exit 1
}

# Step 3: Package
Write-Host "[3/7] Creating deployment package..." -ForegroundColor Yellow
tar -czf deploy_temp.tar.gz -C publish .
Write-Host "  Package created" -ForegroundColor Green

# Step 4: Upload
Write-Host "[4/7] Uploading to server..." -ForegroundColor Yellow
scp -o StrictHostKeyChecking=no -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/root/
Write-Host "  Upload complete" -ForegroundColor Green

# Step 5: Deploy on server
Write-Host "[5/7] Deploying on server..." -ForegroundColor Yellow
ssh -o StrictHostKeyChecking=no -i $SSH_KEY $SERVER "systemctl stop coffee-app.service && rm -rf $DEPLOY_PATH/* && cd $DEPLOY_PATH && tar -xzf /root/deploy_temp.tar.gz && rm /root/deploy_temp.tar.gz"
Write-Host "  Files deployed" -ForegroundColor Green

# Step 6: Apply database migrations (will be done automatically on startup)
Write-Host "[6/7] Database migrations will be applied on startup..." -ForegroundColor Yellow
Write-Host "  (Migrations are now automatic in Program.cs)" -ForegroundColor Green

# Step 7: Copy database and restart
Write-Host "[7/7] Finalizing deployment..." -ForegroundColor Yellow
if (Test-Path "coffee.db") {
    scp -o StrictHostKeyChecking=no -i $SSH_KEY coffee.db ${SERVER}:${DEPLOY_PATH}/
    Write-Host "  Database copied" -ForegroundColor Green
}
ssh -o StrictHostKeyChecking=no -i $SSH_KEY $SERVER "chown -R www-data:www-data $DEPLOY_PATH && systemctl start coffee-app.service"
Write-Host "  Service restarted" -ForegroundColor Green

# Cleanup
Remove-Item deploy_temp.tar.gz -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "================================" -ForegroundColor Cyan
Write-Host "Deployment Complete!" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your app is live at: http://45.55.236.179/coffee/" -ForegroundColor Cyan
Write-Host ""
Write-Host "Check status: ssh -i `"$SSH_KEY`" $SERVER `"systemctl status coffee-app.service`"" -ForegroundColor Gray
