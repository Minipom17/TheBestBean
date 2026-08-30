<div align="center">

# ☕ Purple Bean Coffee

**Premium Peruvian Coffee Platform**

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat-square&logo=bootstrap)](https://getbootstrap.com/)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?style=flat-square&logo=sqlite)](https://www.sqlite.org/)

*Discover exceptional beans, explore origins, and connect with coffee farms worldwide*

[Quick Start](#-quick-start) • [Features](#-features) • [Documentation](#-documentation) • [Deployment](#-deployment)

</div>

---

## 🚀 Quick Start

```powershell
# Stop any running instance
taskkill /F /IM TheBestBean.exe

# Restore & Build
dotnet restore
dotnet build
dotnet run
```

**Local:** http://localhost:5202  
**Production:** http://45.55.236.179/coffee/

### Prerequisites

| Requirement | Version | Link |
|------------|---------|------|
| .NET SDK | 9.0+ | [Download](https://dotnet.microsoft.com/download) |
| Node.js | 18+ | [Download](https://nodejs.org/) |
| npm | Latest | Included with Node.js |

```powershell
# Install dependencies
npm install
dotnet restore
```

---

## 🎨 Features

<table>
<tr>
<td width="50%">

### 📚 Education Hub
- ✅ Flavor Spectrum visualization
- ✅ Brew Methods & Grind Guide
- ✅ Bean Atlas (origins & regions)
- ✅ Roast Science guide
- ✅ Coffee Alchemy (specialty drinks)

</td>
<td width="50%">

### 🛒 E-Commerce
- ✅ Product catalog
- ✅ Session-based cart
- ✅ Checkout system
- ✅ Order confirmation

</td>
</tr>
<tr>
<td width="50%">

### 🌍 Farm Connection
- ✅ Farm profiles with galleries
- ✅ Farmer surveys
- ✅ Interactive coffee map
- ✅ Origin transparency

</td>
<td width="50%">

### 👥 Community
- ✅ Cafe Social events
- ✅ Coffee meetups
- ✅ Educational workshops
- ✅ Virtual tours

</td>
</tr>
</table>

---

## 🛠️ Tech Stack

<div align="center">

| Category | Technology |
|----------|-----------|
| **Framework** | ASP.NET Core 9.0 |
| **Database** | SQLite + Entity Framework Core |
| **Frontend** | Bootstrap 5, Custom CSS |
| **Maps** | Leaflet.js + GeoJSON |
| **Build** | esbuild, npm scripts |
| **Deployment** | Nginx, Systemd, Ubuntu |

</div>

---

## 📁 Project Structure

```
TheBestBean/
├── 📄 Pages/              # UI Pages
│   ├── Index.cshtml
│   ├── Shop.cshtml
│   ├── CoffeeBeans/      # Interactive map
│   ├── FarmProfiles/     # Farm galleries
│   ├── SocialCoffee.cshtml
│   └── Tours.cshtml
├── 🗄️ Models/            # Data models
├── ⚙️ Services/          # Business logic
├── 💾 Data/              # EF Core context
├── 📦 wwwroot/           # Static assets
│   ├── js/              # Bundled JavaScript
│   ├── css/             # Stylesheets
│   ├── data/            # GeoJSON files
│   └── images/          # Uploads & assets
└── 🗺️ ClientApp/        # Source JS
    └── map/             # Coffee map code
```

---

## 📝 Development

### Common Commands

```powershell
# Development
dotnet watch run          # Hot reload
dotnet run                # Standard run

# Build
dotnet build              # C# build
npm run build:map         # JavaScript bundle

# Database
dotnet ef migrations add MigrationName
dotnet ef database update

# Production
dotnet publish -c Release -o publish
```

### JavaScript Workflow

1. Edit `ClientApp/map/coffee-map.js`
2. Run `npm run build:map`
3. Hard refresh browser (Ctrl+F5)

---

## ⚙️ Configuration

### PathBase (Production)

**Critical:** App runs at `/coffee/` subpath. Middleware in `Program.cs`:

```csharp
app.Use((context, next) => {
    var pathBase = context.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();
    if (!string.IsNullOrEmpty(pathBase)) {
        context.Request.PathBase = pathBase;
    } else if (!app.Environment.IsDevelopment()) {
        context.Request.PathBase = "/coffee";
    }
    return next();
});
```

### Map Data

| File | Location | Purpose |
|------|----------|---------|
| World Countries | `wwwroot/data/world_countries.geojson` | Global coffee map |
| Peru Regions | `wwwroot/data/peru_adm1.geojson` | ADM1 level |
| Peru Provinces | `wwwroot/data/peru_adm2.geojson` | ADM2 level |

**API Endpoint:** `/CoffeeBeans/Index?handler=GeoData&level={level}&country={country}`

### Image Uploads

- **Location:** `wwwroot/images/farm_photos/` & `survey_photos/`
- **Formats:** JPG, PNG, GIF
- **Max Size:** 10MB
- **Validation:** File type & size checks enabled

---

## 🗺️ Map Features

<div align="center">

| Feature | Description |
|---------|-------------|
| **Country Colors** | Green (Arabica), Purple (Robusta), Blue (Both) |
| **Regional Data** | ADM1 (regions) → ADM2 (provinces) on click |
| **Smart Scaling** | Markers scale with zoom level |
| **Minimalist Labels** | Coffee bean icons with clean typography |
| **Base Layer** | CartoDB Positron (light, clean) |

</div>

---

## 🚀 Deployment

### Quick Deploy

```powershell
.\deploy_to_production.ps1
```

### Manual Steps

1. **Build:** `dotnet publish -c Release -o publish`
2. **Package:** `tar -czf deploy.tar.gz -C publish .`
3. **Upload:** `scp deploy.tar.gz root@45.55.236.179:/tmp/`
4. **Deploy:** Extract to `/srv/coffee_app/`
5. **Restart:** `systemctl restart coffee-app.service`

### Server Config

- **Nginx:** Reverse proxy to `localhost:5000`
- **PathBase:** `/coffee`
- **Service:** `coffee-app.service` (systemd)
- **User:** `www-data`

---

## 🐛 Troubleshooting

<details>
<summary><b>Map not displaying</b></summary>

- Check browser console for errors
- Verify GeoJSON files exist in `wwwroot/data/`
- Run `npm run build:map`
- Hard refresh (Ctrl+F5)

</details>

<details>
<summary><b>Images not uploading</b></summary>

- Check `wwwroot/images/` permissions
- Verify file size < 10MB
- Check MIME type config in `Program.cs`

</details>

<details>
<summary><b>Navigation broken in production</b></summary>

- Verify PathBase middleware in `Program.cs`
- Check Nginx: `proxy_set_header X-Forwarded-Prefix /coffee;`
- Ensure `proxy_pass` has NO trailing slash

</details>

<details>
<summary><b>Build errors</b></summary>

```powershell
dotnet clean
dotnet restore
# Delete bin/ and obj/ folders
dotnet --version  # Should be 9.0+
```

</details>

---

## 🎨 Design System

<div align="center">

| Element | Value |
|---------|-------|
| **Primary** | `#0059ff` |
| **Accent** | `#ff6b35` |
| **Success** | `#4CAF50` |
| **Charcoal** | `#424242` |
| **Font** | JetBrains Mono, Quicksand |

</div>

**Principles:** Minimalist, clean, professional, coffee-focused

---

## 📊 Data Models

| Model | Purpose |
|-------|---------|
| `FarmProfile` | Farm info, photos, ratings |
| `FarmerSurvey` | Survey data, processing methods |
| `CoffeeBean` | Product catalog |
| `CartItem` | Session cart items |
| `Order` | Checkout & orders |

**Relationships:** `FarmProfile` ↔ `FarmerSurvey` (optional)

---

## 🔐 Security

- ✅ File upload validation (type & size)
- ✅ XSS protection (Razor encoding)
- ✅ CSRF protection (forms)
- ✅ Path sanitization
- ✅ HTTPS via Nginx
- ⚠️ Regular database backups required

---

## 🧪 Testing

### Checklist

- [ ] Map displays with country colors
- [ ] Peru regions load on click
- [ ] Photo uploads work
- [ ] Cart persists during session
- [ ] Navigation works in production
- [ ] Responsive on mobile

### Browsers

✅ Chrome/Edge • Firefox • Safari • Mobile

---

## 📈 Performance

| Aspect | Status |
|--------|--------|
| Static Files | Served by Nginx |
| Database | SQLite (small-medium traffic) |
| JS Bundle | ~161KB (minified) |
| Map Data | GeoJSON (consider lazy loading) |

**Tips:** Use Release builds, compress images, consider CDN

---

## 🔄 Roadmap

### Features
- [ ] User authentication
- [ ] Tour booking system
- [ ] Subscription service
- [ ] Multi-language support
- [ ] Mobile app

### Technical
- [ ] PostgreSQL migration
- [ ] Redis sessions
- [ ] Unit tests
- [ ] CI/CD pipeline
- [ ] Monitoring & logging

---

## 📚 Resources

| Resource | Link |
|----------|------|
| ASP.NET Core | [Docs](https://docs.microsoft.com/aspnet/core) |
| EF Core | [Docs](https://docs.microsoft.com/ef/core) |
| Leaflet.js | [Docs](https://leafletjs.com/reference.html) |
| Bootstrap 5 | [Docs](https://getbootstrap.com/docs/5.3/) |

---

## 💡 Tips

### Adding Pages
1. Create `.cshtml` in `Pages/`
2. Add `.cshtml.cs` PageModel
3. Update navbar in `_Layout.cshtml`

### Database Changes
1. Modify model
2. `dotnet ef migrations add Name`
3. Review migration
4. `dotnet ef database update`

### Styling
- Use Bootstrap utilities
- Custom CSS in `wwwroot/css/coffee-app.css`
- Follow color palette
- Test responsive

---

## 📧 Contact

**Email:** info@llamacoffee.com  
**Issues:** Check Troubleshooting section  
**Features:** Contact development team

---

<div align="center">

**© 2025 Purple Bean Coffee. All rights reserved.**

*From Peru to Your Cup* ☕

</div>
