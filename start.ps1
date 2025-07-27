# ClothingSearch Quick Start Script
# This script starts both backend and frontend for development

param(
    [string]$ProjectPath = "D:\Dev\ClothingSearchApp"
)

Write-Host "=== Starting ClothingSearch Development Environment ===" -ForegroundColor Green
Write-Host ""

# Check if project exists
if (-not (Test-Path $ProjectPath)) {
    Write-Host "❌ Project not found at: $ProjectPath" -ForegroundColor Red
    exit 1
}

Set-Location $ProjectPath

# Start backend
Write-Host "Starting backend API..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$ProjectPath\ClothingSearch.Api'; Write-Host 'Backend API Starting...' -ForegroundColor Green; dotnet run"

# Wait for backend to start
Write-Host "Waiting for backend to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Start frontend
Write-Host "Starting frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Set-Location '$ProjectPath\ClothingSearch.Frontend'; Write-Host 'Frontend Starting...' -ForegroundColor Green; ionic serve"

Write-Host ""
Write-Host "🚀 Development environment started!" -ForegroundColor Green
Write-Host ""
Write-Host "URLs:" -ForegroundColor Yellow
Write-Host "📱 Frontend: http://localhost:8100" -ForegroundColor Cyan
Write-Host "🔌 Backend API: https://localhost:7147" -ForegroundColor Cyan
Write-Host "📖 API Docs: https://localhost:7147/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to open browser windows..."
Read-Host

# Open browser windows
Start-Process "http://localhost:8100"
Start-Process "https://localhost:7147/swagger"

Write-Host "✅ Development environment ready!" -ForegroundColor Green
