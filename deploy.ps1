# ClothingSearch App - Deployment Script
# Deploy to Railway or other hosting platform

param(
    [string]$Environment = "staging",
    [switch]$Build = $false,
    [switch]$Deploy = $false
)

$AppPath = "D:\Dev\ClothingSearchApp"  # Updated to your actual path

Write-Host "🚀 ClothingSearch Deployment Script" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow

# Change to app directory
Set-Location $AppPath

if ($Build) {
    Write-Host "📦 Building application..." -ForegroundColor Green
    
    # Install dependencies
    Write-Host "Installing dependencies..."
    npm install
    
    # Build frontend
    Write-Host "Building frontend..."
    ionic build --prod
    
    # Build backend
    Write-Host "Building backend..."
    dotnet publish -c Release -o ./dist/backend
    
    Write-Host "✅ Build completed successfully!" -ForegroundColor Green
}

if ($Deploy) {
    Write-Host "🚀 Deploying to $Environment..." -ForegroundColor Green
    
    if ($Environment -eq "railway") {
        # Deploy to Railway
        railway deploy
    } elseif ($Environment -eq "vercel") {
        # Deploy frontend to Vercel
        vercel --prod
    } else {
        Write-Host "❌ Unknown environment: $Environment" -ForegroundColor Red
        Write-Host "Supported environments: railway, vercel" -ForegroundColor Yellow
    }
    
    Write-Host "✅ Deployment completed!" -ForegroundColor Green
}

if (-not $Build -and -not $Deploy) {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\deploy.ps1 -Build                    # Build the application"
    Write-Host "  .\deploy.ps1 -Deploy -Environment railway   # Deploy to Railway"
    Write-Host "  .\deploy.ps1 -Build -Deploy           # Build and deploy"
}
