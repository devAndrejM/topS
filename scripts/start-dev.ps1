# Start both backend and frontend for development
Write-Host "Starting ClothingSearch development environment..." -ForegroundColor Green

# Start backend in background
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd ClothingSearch.Api; dotnet run"

# Wait a moment for backend to start
Start-Sleep -Seconds 3

# Start frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd ClothingSearch.Frontend; ionic serve"

Write-Host "Development servers started!" -ForegroundColor Green
Write-Host "Backend: https://localhost:7147" -ForegroundColor Cyan
Write-Host "Frontend: http://localhost:8100" -ForegroundColor Cyan
