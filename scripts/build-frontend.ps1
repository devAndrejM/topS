# Frontend Build Script for Production

param(
    [string]$ProjectPath = "D:\Dev\ClothingSearchApp"
)

Set-Location "$ProjectPath\ClothingSearch.Frontend"
Write-Host "Building frontend for production..." -ForegroundColor Green

try {
    # Install dependencies if needed
    if (!(Test-Path "node_modules")) {
        Write-Host "Installing dependencies..." -ForegroundColor Yellow
        npm install
    }

    # Build for production
    Write-Host "Building production bundle..." -ForegroundColor Yellow
    ionic build --prod

    Write-Host "✅ Frontend build completed!" -ForegroundColor Green
    Write-Host "Output directory: www/" -ForegroundColor Cyan
    
    # Show build info
    $buildPath = "www"
    if (Test-Path $buildPath) {
        $files = Get-ChildItem $buildPath -Recurse | Measure-Object -Property Length -Sum
        $sizeInMB = [math]::Round($files.Sum / 1MB, 2)
        Write-Host "Build size: $sizeInMB MB" -ForegroundColor Cyan
        Write-Host "Files: $($files.Count)" -ForegroundColor Cyan
    }
    
} catch {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
