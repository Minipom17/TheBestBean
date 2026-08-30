# WSL-based deployment script using rsync for The Best Bean Coffee App
# Usage: .\deploy-wsl.ps1

Write-Host "Starting WSL-based deployment to production server..." -ForegroundColor Green

# Build the project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet publish -c Release -o publish --force

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green

# Deploy using WSL rsync
Write-Host "Deploying to production server via WSL rsync..." -ForegroundColor Yellow

$wslCommand = @"
rsync -avz --delete --progress -e "ssh -i /mnt/c/Users/alext/Coffe_Cacoa_Coca.txt" /mnt/c/Users/alext/source/repos/TheBestBean/TheBestBean/publish/ root@45.55.236.179:/srv/coffee_app/
"@

wsl -e bash -c $wslCommand

if ($LASTEXITCODE -ne 0) {
    Write-Host "Rsync deployment failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Files deployed successfully!" -ForegroundColor Green

# Copy database
Write-Host "Copying database..." -ForegroundColor Yellow
scp -i "C:\Users\alext\Coffe_Cacoa_Coca.txt" coffee.db root@45.55.236.179:/srv/coffee_app/

if ($LASTEXITCODE -ne 0) {
    Write-Host "Database copy failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Database copied successfully!" -ForegroundColor Green

# Restart service
Write-Host "Restarting coffee app service..." -ForegroundColor Yellow
ssh -i "C:\Users\alext\Coffe_Cacoa_Coca.txt" root@45.55.236.179 "chown -R www-data:www-data /srv/coffee_app && systemctl restart coffee-app.service"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Service restart failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Deployment complete! Your app is live at: http://45.55.236.179/coffee" -ForegroundColor Cyan