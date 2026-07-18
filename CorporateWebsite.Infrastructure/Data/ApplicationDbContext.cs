using CorporateWebsite.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CorporateWebsite.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Languages
    public DbSet<Language> Languages => Set<Language>();

    // Pages
    public DbSet<Page> Pages => Set<Page>();
    public DbSet<PageTranslation> PageTranslations => Set<PageTranslation>();

    // Services
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceTranslation> ServiceTranslations => Set<ServiceTranslation>();
    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceCategoryTranslation> ServiceCategoryTranslations => Set<ServiceCategoryTranslation>();
    public DbSet<ServiceFeature> ServiceFeatures => Set<ServiceFeature>();
    public DbSet<ServiceFeatureTranslation> ServiceFeatureTranslations => Set<ServiceFeatureTranslation>();
    public DbSet<ServiceFaq> ServiceFaqs => Set<ServiceFaq>();
    public DbSet<ServiceFaqTranslation> ServiceFaqTranslations => Set<ServiceFaqTranslation>();
    public DbSet<ServiceImage> ServiceImages => Set<ServiceImage>();

    // Projects
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTranslation> ProjectTranslations => Set<ProjectTranslation>();
    public DbSet<ProjectCategory> ProjectCategories => Set<ProjectCategory>();
    public DbSet<ProjectCategoryTranslation> ProjectCategoryTranslations => Set<ProjectCategoryTranslation>();
    public DbSet<ProjectImage> ProjectImages => Set<ProjectImage>();
    public DbSet<ProjectVideo> ProjectVideos => Set<ProjectVideo>();
    public DbSet<ProjectFile> ProjectFiles => Set<ProjectFile>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TagTranslation> TagTranslations => Set<TagTranslation>();
    public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();
    public DbSet<ProjectFeature> ProjectFeatures => Set<ProjectFeature>();
    public DbSet<ProjectFeatureTranslation> ProjectFeatureTranslations => Set<ProjectFeatureTranslation>();
    public DbSet<ProjectTeamMember> ProjectTeamMembers => Set<ProjectTeamMember>();
    public DbSet<ProjectTeamMemberTranslation> ProjectTeamMemberTranslations => Set<ProjectTeamMemberTranslation>();

    // News
    public DbSet<News> News => Set<News>();
    public DbSet<NewsTranslation> NewsTranslations => Set<NewsTranslation>();
    public DbSet<NewsCategory> NewsCategories => Set<NewsCategory>();
    public DbSet<NewsCategoryTranslation> NewsCategoryTranslations => Set<NewsCategoryTranslation>();
    public DbSet<NewsTag> NewsTags => Set<NewsTag>();
    public DbSet<NewsImage> NewsImages => Set<NewsImage>();
    public DbSet<NewsVideo> NewsVideos => Set<NewsVideo>();
    public DbSet<NewsFile> NewsFiles => Set<NewsFile>();
    public DbSet<RelatedNews> RelatedNews => Set<RelatedNews>();
    public DbSet<NewsComment> NewsComments => Set<NewsComment>();

    // Menus
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<MenuTranslation> MenuTranslations => Set<MenuTranslation>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemTranslation> MenuItemTranslations => Set<MenuItemTranslation>();

    // Settings
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<SettingTranslation> SettingTranslations => Set<SettingTranslation>();

    // Forms
    public DbSet<Form> Forms => Set<Form>();
    public DbSet<FormTranslation> FormTranslations => Set<FormTranslation>();
    public DbSet<FormField> FormFields => Set<FormField>();
    public DbSet<FormFieldTranslation> FormFieldTranslations => Set<FormFieldTranslation>();
    public DbSet<FormSubmission> FormSubmissions => Set<FormSubmission>();
    public DbSet<FormFieldValue> FormFieldValues => Set<FormFieldValue>();
    public DbSet<FormSubmissionFile> FormSubmissionFiles => Set<FormSubmissionFile>();

    // Sliders/Banners
    public DbSet<Slider> Sliders => Set<Slider>();
    public DbSet<SliderTranslation> SliderTranslations => Set<SliderTranslation>();
    public DbSet<SliderItem> SliderItems => Set<SliderItem>();
    public DbSet<SliderItemTranslation> SliderItemTranslations => Set<SliderItemTranslation>();
    public DbSet<Banner> Banners => Set<Banner>();
    public DbSet<BannerTranslation> BannerTranslations => Set<BannerTranslation>();

    // Media
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<MediaFileTranslation> MediaFileTranslations => Set<MediaFileTranslation>();
    public DbSet<MediaFolder> MediaFolders => Set<MediaFolder>();
    public DbSet<MediaFolderTranslation> MediaFolderTranslations => Set<MediaFolderTranslation>();

    // SEO/Redirects
    public DbSet<Redirect> Redirects => Set<Redirect>();
    public DbSet<BrokenLink> BrokenLinks => Set<BrokenLink>();

    // Logs/Tasks
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<ScheduledTask> ScheduledTasks => Set<ScheduledTask>();
    public DbSet<ScheduledTaskLog> ScheduledTaskLogs => Set<ScheduledTaskLog>();

    // Email
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<EmailTemplateTranslation> EmailTemplateTranslations => Set<EmailTemplateTranslation>();
    public DbSet<EmailQueue> EmailQueue => Set<EmailQueue>();

    // Backups
    public DbSet<Backup> Backups => Set<Backup>();

    // Permissions
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Apply all configurations from assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Configure Identity tables
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(2000);
            entity.Property(e => e.TimeZone).HasMaxLength(50);
            entity.Property(e => e.TwoFactorSecret).HasMaxLength(200);
            entity.Property(e => e.BackupCodes).HasMaxLength(2000);
        });

        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<ApplicationUserClaim>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<ApplicationUserLogin>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<ApplicationRoleClaim>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<ApplicationUserToken>(entity =>
        {
            entity.ToTable("UserTokens");
        });

        // Global query filters for soft delete
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(ApplicationDbContext).GetMethod(nameof(SetSoftDeleteFilter), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);
                method?.Invoke(null, new object[] { builder });
            }
        }

        // Seed default data
        SeedData(builder);
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder) where TEntity : BaseEntity
    {
        builder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
    }

    private static void SeedData(ModelBuilder builder)
    {
        // Seed Languages
        builder.Entity<Language>().HasData(
            new Language { Id = 1, Code = "fa", Name = "Persian", NativeName = "فارسی", CultureCode = "fa-IR", IsRtl = true, DisplayOrder = 1, IsActive = true, IsDefault = true, FlagIcon = "🇮🇷", CreatedAt = DateTime.UtcNow },
            new Language { Id = 2, Code = "en", Name = "English", NativeName = "English", CultureCode = "en-US", IsRtl = false, DisplayOrder = 2, IsActive = true, IsDefault = false, FlagIcon = "🇺🇸", CreatedAt = DateTime.UtcNow },
            new Language { Id = 3, Code = "ar", Name = "Arabic", NativeName = "العربية", CultureCode = "ar-SA", IsRtl = true, DisplayOrder = 3, IsActive = true, IsDefault = false, FlagIcon = "🇸🇦", CreatedAt = DateTime.UtcNow }
        );

        // Seed Default Roles
        builder.Entity<ApplicationRole>().HasData(
            new ApplicationRole { Id = 1, Name = "SuperAdmin", NormalizedName = "SUPERADMIN", Description = "Full system access", IsSystem = true, DisplayOrder = 1, ConcurrencyStamp = Guid.NewGuid().ToString() },
            new ApplicationRole { Id = 2, Name = "Admin", NormalizedName = "ADMIN", Description = "Administrative access", IsSystem = true, DisplayOrder = 2, ConcurrencyStamp = Guid.NewGuid().ToString() },
            new ApplicationRole { Id = 3, Name = "Editor", NormalizedName = "EDITOR", Description = "Content management access", IsSystem = true, DisplayOrder = 3, ConcurrencyStamp = Guid.NewGuid().ToString() },
            new ApplicationRole { Id = 4, Name = "Author", NormalizedName = "AUTHOR", Description = "Content creation access", IsSystem = true, DisplayOrder = 4, ConcurrencyStamp = Guid.NewGuid().ToString() },
            new ApplicationRole { Id = 5, Name = "Viewer", NormalizedName = "VIEWER", Description = "Read-only access", IsSystem = true, DisplayOrder = 5, ConcurrencyStamp = Guid.NewGuid().ToString() }
        );

        // Seed Default Menus
        builder.Entity<Menu>().HasData(
            new Menu { Id = 1, Name = "MainMenu", Description = "Main navigation menu", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Id = 2, Name = "FooterMenu", Description = "Footer navigation menu", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Id = 3, Name = "TopMenu", Description = "Top bar menu", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Menu { Id = 4, Name = "SidebarMenu", Description = "Sidebar navigation menu", IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // Seed Default Settings
        var settings = new List<Setting>
        {
            new Setting { Id = 1, Key = "SiteName", Value = "Corporate Website", Group = "General", DataType = "String", Description = "Site name displayed in title and header", IsPublic = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 2, Key = "SiteDescription", Value = "Professional Corporate Website", Group = "General", DataType = "String", Description = "Site description for SEO", IsPublic = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 3, Key = "SiteKeywords", Value = "corporate, business, company", Group = "General", DataType = "String", Description = "Default meta keywords", IsPublic = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 4, Key = "DefaultLanguage", Value = "fa", Group = "General", DataType = "String", Description = "Default language code", IsPublic = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 5, Key = "SupportedLanguages", Value = "fa,en,ar", Group = "General", DataType = "String", Description = "Comma-separated list of supported language codes", IsPublic = true, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 6, Key = "TimeZone", Value = "Asia/Tehran", Group = "General", DataType = "String", Description = "Default timezone", IsPublic = false, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 7, Key = "DateFormat", Value = "yyyy/MM/dd", Group = "General", DataType = "String", Description = "Default date format", IsPublic = false, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 8, Key = "MaintenanceMode", Value = "false", Group = "General", DataType = "Boolean", Description = "Enable maintenance mode", IsPublic = false, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 9, Key = "DefaultMetaTitle", Value = "", Group = "SEO", DataType = "String", Description = "Default meta title for pages", IsPublic = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 10, Key = "DefaultMetaDescription", Value = "", Group = "SEO", DataType = "String", Description = "Default meta description", IsPublic = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 11, Key = "DefaultMetaKeywords", Value = "", Group = "SEO", DataType = "String", Description = "Default meta keywords", IsPublic = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 12, Key = "SitemapEnabled", Value = "true", Group = "SEO", DataType = "Boolean", Description = "Enable automatic sitemap generation", IsPublic = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 13, Key = "RobotsTxt", Value = "User-agent: *\nAllow: /\n\nSitemap: https://{domain}/sitemap.xml", Group = "SEO", DataType = "String", Description = "Robots.txt content", IsPublic = false, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 14, Key = "SmtpHost", Value = "", Group = "Email", DataType = "String", Description = "SMTP server host", IsPublic = false, IsEncrypted = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 15, Key = "SmtpPort", Value = "587", Group = "Email", DataType = "Integer", Description = "SMTP server port", IsPublic = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 16, Key = "SmtpUsername", Value = "", Group = "Email", DataType = "String", Description = "SMTP username", IsPublic = false, IsEncrypted = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 17, Key = "SmtpPassword", Value = "", Group = "Email", DataType = "String", Description = "SMTP password", IsPublic = false, IsEncrypted = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 18, Key = "SmtpEnableSsl", Value = "true", Group = "Email", DataType = "Boolean", Description = "Enable SSL for SMTP", IsPublic = false, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 19, Key = "SmtpFromEmail", Value = "", Group = "Email", DataType = "String", Description = "From email address", IsPublic = false, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 20, Key = "SmtpFromName", Value = "", Group = "Email", DataType = "String", Description = "From name", IsPublic = false, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 21, Key = "RecaptchaSiteKey", Value = "", Group = "Security", DataType = "String", Description = "reCAPTCHA site key", IsPublic = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 22, Key = "RecaptchaSecretKey", Value = "", Group = "Security", DataType = "String", Description = "reCAPTCHA secret key", IsPublic = false, IsEncrypted = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 23, Key = "RateLimitRequests", Value = "100", Group = "Security", DataType = "Integer", Description = "Requests per window", IsPublic = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 24, Key = "RateLimitWindowMinutes", Value = "60", Group = "Security", DataType = "Integer", Description = "Rate limit window in minutes", IsPublic = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 25, Key = "MaxFileSizeMb", Value = "10", Group = "Upload", DataType = "Integer", Description = "Maximum file upload size in MB", IsPublic = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 26, Key = "AllowedImageTypes", Value = "image/jpeg,image/png,image/gif,image/webp,image/avif", Group = "Upload", DataType = "String", Description = "Allowed image MIME types", IsPublic = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 27, Key = "AllowedDocumentTypes", Value = "application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document", Group = "Upload", DataType = "String", Description = "Allowed document MIME types", IsPublic = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 28, Key = "UploadPath", Value = "uploads", Group = "Upload", DataType = "String", Description = "Upload directory path", IsPublic = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 29, Key = "EnableCompression", Value = "true", Group = "Performance", DataType = "Boolean", Description = "Enable response compression", IsPublic = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 30, Key = "EnableCaching", Value = "true", Group = "Performance", DataType = "Boolean", Description = "Enable response caching", IsPublic = false, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 31, Key = "CacheDurationMinutes", Value = "60", Group = "Performance", DataType = "Integer", Description = "Cache duration in minutes", IsPublic = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 32, Key = "EnableMinification", Value = "true", Group = "Performance", DataType = "Boolean", Description = "Enable CSS/JS minification", IsPublic = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 33, Key = "EnableImageOptimization", Value = "true", Group = "Performance", DataType = "Boolean", Description = "Enable automatic image optimization", IsPublic = false, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 34, Key = "ImageQuality", Value = "80", Group = "Performance", DataType = "Integer", Description = "Image compression quality (1-100)", IsPublic = false, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 35, Key = "WebPEnabled", Value = "true", Group = "Performance", DataType = "Boolean", Description = "Enable WebP conversion", IsPublic = false, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 36, Key = "PrimaryColor", Value = "#2563eb", Group = "Appearance", DataType = "Color", Description = "Primary theme color", IsPublic = true, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 37, Key = "SecondaryColor", Value = "#64748b", Group = "Appearance", DataType = "Color", Description = "Secondary theme color", IsPublic = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 38, Key = "FontFamily", Value = "'Vazirmatn', sans-serif", Group = "Appearance", DataType = "String", Description = "Default font family", IsPublic = true, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
            new Setting { Id = 39, Key = "FontFamilyRtl", Value = "'Vazirmatn', sans-serif", Group = "Appearance", DataType = "String", Description = "RTL font family", IsPublic = true, DisplayOrder = 4, CreatedAt = DateTime.UtcNow }
        };

        builder.Entity<Setting>().HasData(settings);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}