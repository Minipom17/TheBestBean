@echo off
echo 🚀 The Best Bean Coffee App - Quick Deploy
echo ==========================================

echo.
echo 📋 This will:
echo   1. Set up .NET Core on your server (first time only)
echo   2. Build and deploy your coffee app
echo   3. Configure Nginx for both apps
echo.

echo 🔍 Checking if server setup is needed...
ssh -i "C:\Users\alext\Coffe_Cacoa_Coca.txt" -o StrictHostKeyChecking=no root@45.55.236.179 "which dotnet > /dev/null 2>&1 && echo 'setup_done' || echo 'setup_needed'"

echo.
echo 📡 Running server setup (if needed)...
python deploy_coffee_app.py

echo.
echo 🔨 Building and deploying application...
python deploy_coffee_app_build.py

echo.
echo ✅ Deployment complete!
echo 🌐 Test your apps:
echo    - Django: http://45.55.236.179/esl/
echo    - Coffee: http://45.55.236.179/coffee/
echo.

pause



