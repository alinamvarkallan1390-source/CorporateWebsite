using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Application.Services;
using CorporateWebsite.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using CorporateWebsite.Application.Mapping;

namespace CorporateWebsite.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Add AutoMapper
        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        // Add Application Services
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IServiceService, ServiceService>();
        services.AddScoped<IServiceCategoryService, ServiceCategoryService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectCategoryService, ProjectCategoryService>();
        services.AddScoped<INewsService, NewsService>();
        services.AddScoped<INewsCategoryService, NewsCategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<ISettingService, SettingService>();
        services.AddScoped<IFormService, FormService>();
        services.AddScoped<ISliderService, SliderService>();
        services.AddScoped<IBannerService, BannerService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IRedirectService, RedirectService>();
        services.AddScoped<ISeoService, SeoService>();
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IScheduledTaskService, ScheduledTaskService>();
        services.AddScoped<ISearchService, SearchService>();

        return services;
    }
}