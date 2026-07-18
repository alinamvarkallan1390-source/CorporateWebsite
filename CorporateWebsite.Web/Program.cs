using CorporateWebsite.Application;
using CorporateWebsite.Infrastructure;
using CorporateWebsite.Web.Middleware;
using CorporateWebsite.Web.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Globalization;
using LazZiya.ExpressLocalization;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// Add ExpressLocalization for multi-language support
builder.Services.AddExpressLocalization<ExpressLocalizationResource, ExpressLocalizationCultureProvider>(ops =>
{
    ops.ResourcesPath = "Resources";
    ops.RequestLocalizationOptions = o =>
    {
        var supportedCultures = new[]
        {
            new CultureInfo("fa-IR"),
            new CultureInfo("en-US"),
            new CultureInfo("ar-SA")
        };

        o.DefaultRequestCulture = new RequestCulture("fa-IR");
        o.SupportedCultures = supportedCultures;
        o.SupportedUICultures = supportedCultures;
        
        // Custom culture provider for URL-based culture detection
        o.RequestCultureProviders.Insert(0, new ExpressLocalizationCultureProvider(o));
    };
});

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Add Memory Cache
builder.Services.AddMemoryCache();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.AddPolicy("api", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.AddPolicy("form", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(5)
            }));
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add Application and Infrastructure services
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// Add custom services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<ISeoHelperService, SeoHelperService>();
builder.Services.AddScoped<ISchemaHelperService, SchemaHelperService>();
builder.Services.AddScoped<IBreadcrumbService, BreadcrumbService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IImageUrlService, ImageUrlService>();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "XSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Configure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
});

// Add HSTS
builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);
});

// Add Security Headers
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 year
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
    }
});

// Serve uploaded files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
    }
});

app.UseRouting();
app.UseCookiePolicy();
app.UseSession();
app.UseRequestLocalization();
app.UseRateLimiter();

// Custom middleware
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<LanguageRoutingMiddleware>();
app.UseMiddleware<SeoMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "localized",
    pattern: "{culture:regex(^(fa|en|ar)$)}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CorporateWebsite.Infrastructure.Data.ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var dbProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
        
        if (dbProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            logger.LogInformation("Using InMemory database - ensuring created...");
            await context.Database.EnsureCreatedAsync();
        }
        else
        {
            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();
        }
        
        logger.LogInformation("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();