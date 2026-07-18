# Corporate Website - Multi-language ASP.NET Core 8.0

A professional, multi-language (Persian, English, Arabic) corporate website built with ASP.NET Core 8.0, featuring a clean architecture, comprehensive CMS capabilities, and enterprise-grade features.

## 🚀 Features

### Core Features
- **Multi-language Support**: Persian (RTL), English (LTR), Arabic (RTL) with full localization
- **Clean Architecture**: Separation of concerns with Core, Application, Infrastructure, and Web layers
- **Entity Framework Core 8.0**: Code-first approach with migrations
- **ASP.NET Core Identity**: Role-based authorization with custom permissions
- **Responsive Design**: Mobile-first approach with Bootstrap 5 RTL support

### Content Management
- **Dynamic Pages**: Hierarchical pages with SEO optimization
- **Services Management**: Categories, features, FAQs, galleries
- **Projects/Portfolio**: Categories, tags, team members, media
- **News/Blog**: Categories, tags, scheduled publishing, comments
- **Media Library**: Folder-based organization with image optimization
- **Forms Builder**: Dynamic forms with validation, file uploads, email notifications
- **Sliders & Banners**: Multi-language with scheduling

### SEO & Performance
- **Technical SEO**: Sitemap.xml, robots.txt, canonical URLs, hreflang
- **Schema.org**: Organization, WebSite, Article, Service, FAQPage, BreadcrumbList, VideoObject
- **Open Graph & Twitter Cards**: Full social media optimization
- **Core Web Vitals**: Lazy loading, image optimization (WebP/AVIF), compression, caching
- **Lighthouse Ready**: Optimized for performance, accessibility, SEO

### Security
- **Authentication**: ASP.NET Core Identity with 2FA support
- **Authorization**: Granular permission system
- **Protection**: CSRF, XSS, SQL Injection, Rate Limiting
- **Headers**: CSP, HSTS, Security Headers middleware
- **reCAPTCHA**: v2/v3 support for forms

### Admin Panel
- **Dashboard**: Statistics and quick actions
- **Content Management**: Full CRUD for all entities
- **Media Manager**: Drag-drop upload, folders, image editing
- **User Management**: Roles, permissions, activity logs
- **Settings**: SEO, Security, Appearance, Email, SMTP
- **Redirects**: 301/302 management, broken link tracking
- **Backups**: Automated and manual backup/restore
- **Scheduled Tasks**: Background jobs with cron expressions

### Technical Stack
- **Framework**: ASP.NET Core 8.0 LTS
- **Database**: SQL Server / PostgreSQL
- **ORM**: Entity Framework Core 8.0
- **Frontend**: Bootstrap 5 RTL, Vanilla JS (ES6+)
- **Localization**: LazZiya.ExpressLocalization
- **Logging**: Serilog (Console + File)
- **Image Processing**: SixLabors.ImageSharp
- **Export**: ClosedXML (Excel), CSV
- **Email**: MailKit (SMTP)
- **Containerization**: Docker, Docker Compose
- **Reverse Proxy**: Nginx with SSL

## 📁 Project Structure

```
CorporateWebsite/
├── CorporateWebsite.Core/           # Domain Entities & Interfaces
│   ├── Entities/                    # Domain Models
│   ├── DTOs/                        # Data Transfer Objects
│   └── Interfaces/                  # Repository Interfaces
├── CorporateWebsite.Application/    # Business Logic
│   ├── Services/                    # Application Services
│   ├── Interfaces/                  # Service Interfaces
│   ├── Mapping/                     # AutoMapper Profiles
│   └── DependencyInjection.cs
├── CorporateWebsite.Infrastructure/ # Data Access & External Services
│   ├── Data/                        # DbContext & Migrations
│   ├── Repositories/                # Repository Implementations
│   ├── Services/                    # Infrastructure Services
│   └── DependencyInjection.cs
├── CorporateWebsite.Web/            # Presentation Layer
│   ├── Controllers/                 # MVC Controllers
│   ├── ViewComponents/              # Reusable UI Components
│   ├── Middleware/                  # Custom Middleware
│   ├── Services/                    # Web-specific Services
│   ├── Views/                       # Razor Views
│   ├── wwwroot/                     # Static Files
│   └── Resources/                   # Localization Resources
├── Dockerfile
├── docker-compose.yml
├── nginx.conf
└── README.md
```

## 🛠️ Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+ or PostgreSQL 14+
- Docker & Docker Compose (optional)
- Node.js 18+ (for frontend build if needed)

### Local Development

1. **Clone the repository**
```bash
git clone https://github.com/yourusername/CorporateWebsite.git
cd CorporateWebsite
```

2. **Configure Database**
Edit `CorporateWebsite.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CorporateWebsite;User=sa;Password=YourPassword;TrustServerCertificate=true;"
  }
}
```

3. **Run Migrations**
```bash
cd CorporateWebsite.Web
dotnet ef database update
```

4. **Run the Application**
```bash
dotnet run
```

5. **Access the Website**
- Frontend: https://localhost:5001
- Admin: https://localhost:5001/admin
- Default admin: `admin@corporate.com` / `Admin@123`

### Docker Deployment

1. **Configure Environment**
```bash
cp .env.example .env
# Edit .env with your settings
```

2. **Build and Run**
```bash
docker-compose up -d --build
```

3. **Run Migrations**
```bash
docker-compose exec web dotnet ef database update
```

### Production Deployment

1. **SSL Certificates**
Place your SSL certificates in `./ssl/`:
- `fullchain.pem`
- `privkey.pem`

2. **Configure Nginx**
Update `nginx.conf` with your domain name.

3. **Deploy**
```bash
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## 🌐 Multi-language Setup

### Adding New Language
1. Add language in Admin Panel → Languages
2. Create resource files in `Resources/Views/`
3. Translate all content through Admin Panel

### URL Structure
- Persian (default): `https://domain.com/fa/`
- English: `https://domain.com/en/`
- Arabic: `https://domain.com/ar/`

### RTL Support
Automatic RTL/LTR switching based on language:
- Persian & Arabic: RTL
- English: LTR

## 🔧 Configuration

### Key Settings (Admin Panel → Settings)

| Group | Key | Description |
|-------|-----|-------------|
| General | SiteName | Site name for title/header |
| General | DefaultLanguage | Default language code (fa/en/ar) |
| SEO | DefaultMetaTitle | Default meta title |
| SEO | GoogleSiteVerification | Google Search Console code |
| Email | SmtpHost/SmtpPort | SMTP server settings |
| Security | RecaptchaSiteKey/SecretKey | reCAPTCHA keys |
| Upload | MaxFileSizeMb | Max upload size |
| Performance | EnableImageOptimization | Auto WebP conversion |

## 📝 Development Guidelines

### Adding New Entity
1. Create entity in `Core/Entities/`
2. Add DTOs in `Core/DTOs/`
3. Create repository interface in `Core/Interfaces/`
4. Implement repository in `Infrastructure/Repositories/`
5. Create service interface in `Application/Interfaces/`
6. Implement service in `Application/Services/`
7. Register in DI containers
8. Create Admin Controller & Views
9. Add migration: `dotnet ef migrations add AddNewEntity`
10. Update database: `dotnet ef database update`

### Code Style
- Follow C# coding conventions
- Use async/await throughout
- Implement proper error handling
- Add XML documentation for public APIs
- Write unit tests for business logic

### Git Workflow
1. Create feature branch from `develop`
2. Implement changes with tests
3. Create Pull Request
4. Code review required
5. Merge after approval

## 🧪 Testing

```bash
# Run unit tests
dotnet test CorporateWebsite.Tests/CorporateWebsite.Tests.csproj

# Run integration tests
dotnet test CorporateWebsite.IntegrationTests/CorporateWebsite.IntegrationTests.csproj

# Coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📦 Build & Publish

```bash
# Build Release
dotnet build -c Release

# Publish
dotnet publish CorporateWebsite.Web/CorporateWebsite.Web.csproj -c Release -o ./publish

# Docker Build
docker build -t corporatewebsite:latest .
```

## 🚀 CI/CD Pipeline

The project includes GitHub Actions workflows for:
- ✅ Build & Test on every push
- ✅ Docker image build on release
- ✅ Security scanning
- ✅ Deployment to staging/production

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create feature branch
3. Commit changes
4. Push to branch
5. Create Pull Request

## 📞 Support

For support, email support@corporatewebsite.com or create an issue on GitHub.

## 🙏 Acknowledgments

- ASP.NET Core Team
- Bootstrap Team
- SixLabors.ImageSharp
- Serilog
- MailKit
- All open-source contributors