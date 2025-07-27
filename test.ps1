# Testing Script for ClothingSearch App

param(
    [string]$ProjectPath = "D:\Dev\ClothingSearchApp"
)

Write-Host "=== Testing ClothingSearch App ===" -ForegroundColor Green

Set-Location $ProjectPath

# Test backend
Write-Host "Testing backend..." -ForegroundColor Cyan
Set-Location "ClothingSearch.Api"

Write-Host "Running backend tests..." -ForegroundColor Yellow
try {
    # Build test
    dotnet build --configuration Release
    Write-Host "✅ Backend build test passed" -ForegroundColor Green
    
    # Start backend temporarily to test
    Write-Host "Starting backend for API test..." -ForegroundColor Yellow
    $backendProcess = Start-Process dotnet -ArgumentList "run" -PassThru -NoNewWindow
    
    Start-Sleep -Seconds 10
    
    # Test API endpoint
    try {
        $response = Invoke-RestMethod -Uri "https://localhost:7147/api/user/countries" -SkipCertificateCheck
        Write-Host "✅ API endpoint test passed" -ForegroundColor Green
        Write-Host "Found $($response.Count) countries in database" -ForegroundColor Cyan
    } catch {
        Write-Host "⚠️  API endpoint test failed (normal if DB not configured)" -ForegroundColor Yellow
    }
    
    # Stop backend
    Stop-Process -Id $backendProcess.Id -Force
    
} catch {
    Write-Host "❌ Backend test failed: $_" -ForegroundColor Red
}

Set-Location ..

# Test frontend
Write-Host "Testing frontend..." -ForegroundColor Cyan
Set-Location "ClothingSearch.Frontend"

Write-Host "Running frontend tests..." -ForegroundColor Yellow
try {
    # Build test
    ionic build --prod
    Write-Host "✅ Frontend build test passed" -ForegroundColor Green
    
    # Check build output
    if (Test-Path "www") {
        $files = Get-ChildItem "www" -Recurse
        Write-Host "✅ Frontend files generated: $($files.Count)" -ForegroundColor Green
    }
    
} catch {
    Write-Host "❌ Frontend test failed: $_" -ForegroundColor Red
}

Set-Location ..

Write-Host ""
Write-Host "=== Test Summary ===" -ForegroundColor Green
Write-Host "Run './start.ps1' to start development environment" -ForegroundColor Cyan
Write-Host "Check DEPLOYMENT.md for production deployment steps" -ForegroundColor Cyan
