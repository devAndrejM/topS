<<<<<<< HEAD
﻿# ClothingSearch App

Multi-store clothing search aggregator that searches multiple retailers simultaneously.

## 🏗️ Tech Stack
- **Backend**: .NET 8 Web API + Entity Framework Core
- **Frontend**: Ionic 7 + Angular 17
- **Database**: In-Memory SQLite (development) → PostgreSQL (production)

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Ionic CLI (
pm install -g @ionic/cli)

### Development Setup

1. **Start Backend (Terminal 1)**
`powershell
cd ClothingSearch.Api
dotnet run
# Runs on: http://localhost:5000
`

2. **Start Frontend (Terminal 2)**
`powershell
cd ClothingSearch.Frontend
npm install
ionic serve
# Runs on: http://localhost:8100
`

### API Documentation
- Swagger UI: http://localhost:5000/swagger

## 📊 Features
- ✅ Text-based product search across multiple stores
- ✅ Smart category filtering
- ✅ User preference management
- ✅ Affiliate link integration ready
- ✅ Responsive design
- ✅ Croatian/English i18n support

## 🔍 Search Flow
1. User searches for "Nike Air Max"
2. API searches configured stores (Amazon, Hervis mock providers)
3. Results grouped by category: "Shoes (4) | Clothing (2)"
4. User clicks category to filter results
5. Results cached for 6 hours

## 🌍 Target Markets
- **Phase 1**: Croatia (Hervis, local stores)
- **Phase 2**: EU expansion (Zalando, ASOS)
- **Phase 3**: Global (Amazon, major retailers)

## 💰 Revenue Model
Commission-based affiliate links (3-8% per sale)

---
**Status**: 🟢 MVP Complete - Ready for real store integration
=======
# topS
>>>>>>> 30225e9e59151c5295da47c9645d27089c590f25
