# Build both projects for production
Write-Host "Building ClothingSearch for production..." -ForegroundColor Green

# Build backend
Write-Host "Building API..." -ForegroundColor Yellow
Set-Location "ClothingSearch.Api"
dotnet publish -c Release -o ../publish/api

# Build frontend
Write-Host "Building Frontend..." -ForegroundColor Yellow
Set-Location "../ClothingSearch.Frontend"
ionic build --prod

Write-Host "Build completed!" -ForegroundColor Green
Write-Host "API output: publish/api/" -ForegroundColor Cyan
Write-Host "Frontend output: ClothingSearch.Frontend/dist/" -ForegroundColor Cyan
