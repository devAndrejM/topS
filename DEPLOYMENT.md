# ClothingSearch Deployment Checklist

## Prerequisites
- [x] Node.js 18+
- [x] .NET 8 SDK  
- [x] Angular CLI
- [ ] Ionic CLI (optional: `npm install -g @ionic/cli`)
- [ ] Railway CLI: `npm install -g @railway/cli`

## Local Development Setup
- [x] Project structure created
- [x] Backend API with EF Core
- [x] Frontend Ionic app
- [x] Database models and migrations
- [x] Search functionality
- [x] User settings
- [x] Build configuration fixed
- [x] Environment files created

## Next Steps for Deployment

### 1. Database Setup (Choose one)

#### Option A: Railway PostgreSQL (Recommended)
```bash
railway login
railway new clothingsearch-app
railway add postgresql
railway variables  # Copy DATABASE_URL
```

#### Option B: Local PostgreSQL
```powershell
./database/setup-local-postgres.ps1
```

### 2. Update Configuration

#### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_RAILWAY_CONNECTION_STRING"
  }
}
```

#### Frontend (environment.prod.ts)
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.railway.app'
};
```

### 3. Deploy to Railway

#### Deploy Backend
```bash
cd ClothingSearch.Api
railway up
```

#### Deploy Frontend
```bash
cd ClothingSearch.Frontend
ng build --configuration=production
# or: ionic build --prod (if Ionic CLI installed)
railway up
```

### 4. Configure Environment Variables in Railway
- ASPNETCORE_ENVIRONMENT=Production
- ASPNETCORE_URLS=http://0.0.0.0:8080

### 5. Test Deployment
- [ ] API health check
- [ ] Database connection
- [ ] Frontend loads
- [ ] Search functionality
- [ ] Settings save/load

## Build Commands Reference

### Frontend Build Options
```powershell
# Angular CLI (what you're currently using)
ng build --configuration=production

# Development build (faster, larger files)
ng build --configuration=development

# Ionic CLI (if installed)
ionic build --prod
```

### Frontend Serve Options
```powershell
# Angular CLI (port 4200)
ng serve

# Ionic CLI (port 8100, if installed)
ionic serve
```

## Affiliate Networks Setup

### Priority 1: Get approved for affiliate programs
1. **Amazon Associates** - amazon.com/associates
2. **TradeTracker** - tradetracker.com (EU focused)
3. **Zalando Partner Program** - zalando.com/partner
4. **ASOS Affiliate Program** - asos.com/partners

### Priority 2: Implement APIs
1. Replace mock data in providers
2. Add real affiliate tracking
3. Implement product caching
4. Add error handling

## Store Integration Roadmap

### Phase 1 (MVP) - 2 weeks
- [x] Basic project structure
- [x] Search interface
- [x] User settings
- [x] Build system working
- [ ] Deploy to Railway
- [ ] 2-3 store integrations

### Phase 2 - 4 weeks  
- [ ] Real affiliate APIs
- [ ] Croatian stores (Hervis, Intersport)
- [ ] Enhanced search filters
- [ ] Performance optimization

### Phase 3 - 8 weeks
- [ ] More stores (ASOS, Zalando)
- [ ] Advanced features
- [ ] Analytics dashboard
- [ ] User feedback system

## Performance Targets
- [ ] Search results < 3 seconds
- [ ] Page load < 2 seconds
- [ ] Mobile responsive
- [ ] 95% uptime

## Marketing & Growth
- [ ] Croatian fashion blogs outreach
- [ ] Social media presence
- [ ] SEO optimization
- [ ] User feedback collection