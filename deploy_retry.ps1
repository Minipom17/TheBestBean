$SSH_KEY = "C:\Users\alext\Coffe_Cacoa_Coca.txt"
$SERVER = "root@45.55.236.179"
$DEPLOY_PATH = "/srv/coffee_app"

Write-Host "Uploading deploy_temp.tar.gz..."
$success = $false
$retries = 0
while (-not $success -and $retries -lt 3) {
    scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY deploy_temp.tar.gz ${SERVER}:/root/
    if ($LASTEXITCODE -eq 0) {
        $success = $true
        Write-Host "Upload succeeded."
    } else {
        $retries++
        Write-Host "Upload failed. Retrying... ($retries/3)"
        Start-Sleep -Seconds 2
    }
}

if (-not $success) {
    Write-Host "Failed to upload tarball after 3 retries."
    exit 1
}

Write-Host "Uploading coffee.db..."
scp -o StrictHostKeyChecking=no -o ConnectTimeout=30 -i $SSH_KEY coffee.db ${SERVER}:/root/

Write-Host "Deploying on server..."
ssh -o StrictHostKeyChecking=no -i $SSH_KEY $SERVER "systemctl stop coffee-app.service && rm -rf $DEPLOY_PATH/* && cd $DEPLOY_PATH && tar -xzf /root/deploy_temp.tar.gz && mv /root/coffee.db $DEPLOY_PATH/ && chown -R www-data:www-data $DEPLOY_PATH && systemctl start coffee-app.service"
Write-Host "Deployment complete."
