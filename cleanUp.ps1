# ClothingSearch Project Cleanup Script - SIMPLE VERSION
# RUN FROM: D:\Dev\ClothingSearchApp\ClothingSearch.Frontend

Write-Host "Starting ClothingSearch Project Cleanup..." -ForegroundColor Green

# Verify directory
if (-not (Test-Path "package.json")) {
    Write-Host "Error: Run this from ClothingSearch.Frontend directory" -ForegroundColor Red
    exit 1
}

Write-Host "Correct directory confirmed" -ForegroundColor Green

# Backup
Write-Host "Creating backup commit..." -ForegroundColor Yellow
git add .
git commit -m "Backup before cleanup"

Write-Host "Removing unnecessary files..." -ForegroundColor Cyan

# Remove tab components
if (Test-Path "src/app/tab1") { Remove-Item "src/app/tab1" -Recurse -Force }
if (Test-Path "src/app/tab2") { Remove-Item "src/app/tab2" -Recurse -Force }
if (Test-Path "src/app/tab3") { Remove-Item "src/app/tab3" -Recurse -Force }
if (Test-Path "src/app/tabs") { Remove-Item "src/app/tabs" -Recurse -Force }

# Remove demo components
if (Test-Path "src/app/explore-container") { Remove-Item "src/app/explore-container" -Recurse -Force }

# Remove module files
if (Test-Path "src/app/pages/search/search.module.ts") { Remove-Item "src/app/pages/search/search.module.ts" }
if (Test-Path "src/app/pages/search/search-routing.module.ts") { Remove-Item "src/app/pages/search/search-routing.module.ts" }

# Remove i18n
if (Test-Path "src/assets/i18n") { Remove-Item "src/assets/i18n" -Recurse -Force }

Write-Host "Creating clean routing file..." -ForegroundColor Cyan

# Create app.routes.ts
$routesContent = "import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/search',
    pathMatch: 'full',
  },
  {
    path: 'search',
    loadComponent: () => import('./pages/search/search.page').then((m) => m.SearchPage),
  },
];"

$routesContent | Out-File "src/app/app.routes.ts" -Encoding UTF8

Write-Host "Creating clean app component HTML..." -ForegroundColor Cyan

# Create app.component.html
$appHtml = "<ion-app>
  <ion-router-outlet></ion-router-outlet>
</ion-app>"

$appHtml | Out-File "src/app/app.component.html" -Encoding UTF8

Write-Host "Creating standalone app component..." -ForegroundColor Cyan

# Create app.component.ts
$appComponent = "import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  standalone: true,
  imports: [IonApp, IonRouterOutlet],
})
export class AppComponent {
  constructor() {}
}"

$appComponent | Out-File "src/app/app.component.ts" -Encoding UTF8

Write-Host "Creating standalone main.ts..." -ForegroundColor Cyan

# Create main.ts
$mainTs = "import { bootstrapApplication } from '@angular/platform-browser';
import { RouteReuseStrategy, provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { IonicRouteStrategy, provideIonicAngular } from '@ionic/angular/standalone';

import { routes } from './app/app.routes';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, {
  providers: [
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    provideIonicAngular(),
    provideRouter(routes, withPreloading(PreloadAllModules)),
  ],
});"

$mainTs | Out-File "src/main.ts" -Encoding UTF8

Write-Host ""
Write-Host "Cleanup Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "What was removed:" -ForegroundColor Cyan
Write-Host "- Tab components (tab1, tab2, tab3, tabs)" -ForegroundColor White
Write-Host "- Demo explore-container component" -ForegroundColor White
Write-Host "- Search module files" -ForegroundColor White
Write-Host "- i18n files" -ForegroundColor White
Write-Host ""
Write-Host "What was created:" -ForegroundColor Green
Write-Host "- Clean app.routes.ts" -ForegroundColor White
Write-Host "- Minimal app.component.html" -ForegroundColor White
Write-Host "- Standalone app.component.ts" -ForegroundColor White
Write-Host "- Bootstrap main.ts" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Test: ionic serve" -ForegroundColor White
Write-Host "2. Commit: git add . && git commit -m 'Clean project structure'" -ForegroundColor White
Write-Host "3. Push: git push" -ForegroundColor White
Write-Host ""
Write-Host "Your app now has clean standalone architecture!" -ForegroundColor Green