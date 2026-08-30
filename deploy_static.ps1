# Fast deploy for static files only (images, favicons, css) — no rebuild, no DB touch.
# Usage:
#   .\deploy_static.ps1 images/logoo.svg
#   .\deploy_static.ps1 images/og-logo.png wwwroot/favicon.ico
#   .\deploy_static.ps1 -AllImages

param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Files,
    [switch]$AllImages
)

$SSH_KEY = "C:\Users\alext\Coffe_Cacoa_Coca.txt"
$SERVER = "root@45.55.236.179"
$DEPLOY_PATH = "/srv/coffee_app"
$PROJECT_PATH = "C:\Users\alext\source\repos\TheBestBean\TheBestBean"
$WWWROOT = Join-Path $PROJECT_PATH "wwwroot"

if (-not (Test-Path $SSH_KEY)) {
    Write-Host "SSH key not found: $SSH_KEY" -ForegroundColor Red
    exit 1
}

if ($AllImages) {
    $Files = @(
        "images/logoo.svg",
        "images/og-logo.png",
        "images/og-logo-square.png",
        "images/brand-icon.png",
        "images/favicon-48.png",
        "images/favicon-96.png",
        "images/apple-touch-icon.png",
        "favicon.ico"
    )
}

if (-not $Files -or $Files.Count -eq 0) {
    Write-Host "Usage: .\deploy_static.ps1 images/logoo.svg" -ForegroundColor Yellow
    Write-Host "       .\deploy_static.ps1 -AllImages" -ForegroundColor Yellow
    exit 1
}

$toUpload = @()
foreach ($rel in $Files) {
    $rel = $rel.TrimStart('/', '\')
    $local = Join-Path $WWWROOT $rel
    if (-not (Test-Path $local)) {
        Write-Host "Missing: $local" -ForegroundColor Red
        exit 1
    }
    $toUpload += @{ Local = $local; Rel = $rel }
}

Write-Host "Uploading $($toUpload.Count) file(s)..." -ForegroundColor Cyan
foreach ($item in $toUpload) {
    $remoteDir = Split-Path $item.Rel -Parent
    if ($remoteDir -and $remoteDir -ne '.') {
        ssh -o StrictHostKeyChecking=no -i $SSH_KEY $SERVER "mkdir -p $DEPLOY_PATH/wwwroot/$($remoteDir -replace '\\','/')"
    }
    $remote = "$DEPLOY_PATH/wwwroot/$($item.Rel -replace '\\','/')"
    scp -o StrictHostKeyChecking=no -i $SSH_KEY $item.Local "${SERVER}:$remote"
    Write-Host "  $($item.Rel)" -ForegroundColor Green
}

ssh -o StrictHostKeyChecking=no -i $SSH_KEY $SERVER "chown -R www-data:www-data $DEPLOY_PATH/wwwroot"
Write-Host "Done. No app restart needed for static files." -ForegroundColor Green
