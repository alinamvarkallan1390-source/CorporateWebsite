using CorporateWebsite.Core.Interfaces;
using CorporateWebsite.Infrastructure.Data;
using CorporateWebsite.Infrastructure.Repositories;
using CorporateWebsite.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace CorporateWebsite.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var dbProvider = configuration["Database:Provider"] ?? "SqlServer";

        if (dbProvider.Equals("InMemory", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("CorporateWebsite")
                    .EnableSensitiveDataLogging(configuration.GetValue<bool>("Database:EnableSensitiveDataLogging"))
                    .EnableDetailedErrors(configuration.GetValue<bool>("Database:EnableDetailedErrors")));
        }
        else if (dbProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=localhost;Database=CorporateWebsite;Username=postgres;Password=postgres";
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure();
                })
                .EnableSensitiveDataLogging(configuration.GetValue<bool>("Database:EnableSensitiveDataLogging"))
                .EnableDetailedErrors(configuration.GetValue<bool>("Database:EnableDetailedErrors")));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=(localdb)\\mssqllocaldb;Database=CorporateWebsite;Trusted_Connection=true;MultipleActiveResultSets=true";
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure();
                })
                .EnableSensitiveDataLogging(configuration.GetValue<bool>("Database:EnableSensitiveDataLogging"))
                .EnableDetailedErrors(configuration.GetValue<bool>("Database:EnableDetailedErrors")));
        }

        // Add Identity
        services.AddIdentity<CorporateWebsite.Core.Entities.ApplicationUser, CorporateWebsite.Core.Entities.ApplicationRole>(options =>
        {
            options.Password.RequireDigit = configuration.GetValue<bool>("Security:PasswordRequireDigit", true);
            options.Password.RequireLowercase = configuration.GetValue<bool>("Security:PasswordRequireLowercase", true);
            options.Password.RequireUppercase = configuration.GetValue<bool>("Security:PasswordRequireUppercase", true);
            options.Password.RequireNonAlphanumeric = configuration.GetValue<bool>("Security:PasswordRequireSpecialChar", true);
            options.Password.RequiredLength = configuration.GetValue<int>("Security:PasswordMinLength", 8);
            options.Password.RequiredUniqueChars = configuration.GetValue<int>("Security:PasswordRequiredUniqueChars", 1);

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(configuration.GetValue<int>("Security:LockoutDurationMinutes", 15));
            options.Lockout.MaxFailedAccessAttempts = configuration.GetValue<int>("Security:LockoutMaxFailedAttempts", 5);
            options.Lockout.AllowedForNewUsers = true;

            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>("Security:RequireConfirmedEmail", false);
            options.SignIn.RequireConfirmedPhoneNumber = false;

            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Add Unit of Work and Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add Infrastructure Services
        services.AddScoped<IImageOptimizationService, ImageOptimizationService>();
        services.AddScoped<ISitemapService, SitemapService>();
        services.AddScoped<IRobotsTxtService, RobotsTxtService>();
        services.AddScoped<ISchemaService, SchemaService>();
        services.AddScoped<IFileStorageService, FileStorageService>();

        // Configure Serilog
        ConfigureSerilog(configuration);

        return services;
    }

    private static void ConfigureSerilog(IConfiguration configuration)
    {
        var logPath = configuration["Logging:FilePath"] ?? "logs/log-.txt";
        var logLevel = configuration.GetValue<string>("Logging:LogLevel:Default") ?? "Information";

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "CorporateWebsite")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                fileSizeLimitBytes: 10_000_000)
            .CreateLogger();
    }
}