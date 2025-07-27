# ClothingSearch App - Testing Script
# Run all tests and quality checks

param(
    [switch]$Unit = $false,
    [switch]$E2E = $false,
    [switch]$Lint = $false,
    [switch]$All = $false
)

$AppPath = "C:\dev\clothingsearch"  # Update this to your actual path

Write-Host "🧪 ClothingSearch Testing Script" -ForegroundColor Green

# Change to app directory
Set-Location $AppPath

if ($All) {
    $Unit = $true
    $E2E = $true
    $Lint = $true
}

if ($Lint) {
    Write-Host "🔍 Running linting..." -ForegroundColor Green
    
    # Frontend linting
    Write-Host "Linting frontend..."
    npm run lint
    
    # Backend linting (if using dotnet format)
    Write-Host "Linting backend..."
    dotnet format --verify-no-changes
    
    Write-Host "✅ Linting completed!" -ForegroundColor Green
}

if ($Unit) {
    Write-Host "🧪 Running unit tests..." -ForegroundColor Green
    
    # Frontend unit tests
    Write-Host "Running frontend tests..."
    npm run test:unit
    
    # Backend unit tests
    Write-Host "Running backend tests..."
    dotnet test
    
    Write-Host "✅ Unit tests completed!" -ForegroundColor Green
}

if ($E2E) {
    Write-Host "🔄 Running E2E tests..." -ForegroundColor Green
    
    # Start local server
    Write-Host "Starting local server..."
    Start-Process -FilePath "ionic" -ArgumentList "serve" -NoNewWindow
    
    # Wait for server to start
    Start-Sleep -Seconds 10
    
    # Run E2E tests
    Write-Host "Running E2E tests..."
    npm run test:e2e
    
    Write-Host "✅ E2E tests completed!" -ForegroundColor Green
}

if (-not $Unit -and -not $E2E -and -not $Lint -and -not $All) {
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\test.ps1 -Unit      # Run unit tests"
    Write-Host "  .\test.ps1 -E2E       # Run E2E tests"
    Write-Host "  .\test.ps1 -Lint      # Run linting"
    Write-Host "  .\test.ps1 -All       # Run all tests"
}
