using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CorporateWebsite.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Language> Languages { get; }
    IRepository<Page> Pages { get; }
    IRepository<PageTranslation> PageTranslations { get; }
    IRepository<Service> Services { get; }
    IRepository<ServiceTranslation> ServiceTranslations { get; }
    IRepository<ServiceCategory> ServiceCategories { get; }
    IRepository<ServiceCategoryTranslation> ServiceCategoryTranslations { get; }
    IRepository<Project> Projects { get; }
    IRepository<ProjectTranslation> ProjectTranslations { get; }
    IRepository<ProjectCategory> ProjectCategories { get; }
    IRepository<ProjectCategoryTranslation> ProjectCategoryTranslations { get; }
    IRepository<News> News { get; }
    IRepository<NewsTranslation> NewsTranslations { get; }
    IRepository<NewsCategory> NewsCategories { get; }
    IRepository<NewsCategoryTranslation> NewsCategoryTranslations { get; }
    IRepository<Tag> Tags { get; }
    IRepository<TagTranslation> TagTranslations { get; }
    IRepository<Menu> Menus { get; }
    IRepository<MenuTranslation> MenuTranslations { get; }
    IRepository<MenuItem> MenuItems { get; }
    IRepository<MenuItemTranslation> MenuItemTranslations { get; }
    IRepository<Setting> Settings { get; }
    IRepository<SettingTranslation> SettingTranslations { get; }
    IRepository<Form> Forms { get; }
    IRepository<FormTranslation> FormTranslations { get; }
    IRepository<FormField> FormFields { get; }
    IRepository<FormFieldTranslation> FormFieldTranslations { get; }
    IRepository<FormSubmission> FormSubmissions { get; }
    IRepository<FormFieldValue> FormFieldValues { get; }
    IRepository<FormSubmissionFile> FormSubmissionFiles { get; }
    IRepository<Slider> Sliders { get; }
    IRepository<SliderTranslation> SliderTranslations { get; }
    IRepository<SliderItem> SliderItems { get; }
    IRepository<SliderItemTranslation> SliderItemTranslations { get; }
    IRepository<Banner> Banners { get; }
    IRepository<BannerTranslation> BannerTranslations { get; }
    IRepository<MediaFile> MediaFiles { get; }
    IRepository<MediaFileTranslation> MediaFileTranslations { get; }
    IRepository<MediaFolder> MediaFolders { get; }
    IRepository<MediaFolderTranslation> MediaFolderTranslations { get; }
    IRepository<Redirect> Redirects { get; }
    IRepository<ActivityLog> ActivityLogs { get; }
    IRepository<BrokenLink> BrokenLinks { get; }
    IRepository<ScheduledTask> ScheduledTasks { get; }
    IRepository<ScheduledTaskLog> ScheduledTaskLogs { get; }
    IRepository<EmailTemplate> EmailTemplates { get; }
    IRepository<EmailTemplateTranslation> EmailTemplateTranslations { get; }
    IRepository<EmailQueue> EmailQueue { get; }
    IRepository<Backup> Backups { get; }
    IRepository<RolePermission> RolePermissions { get; }
    IRepository<ProjectTag> ProjectTags { get; }
    IRepository<NewsTag> NewsTags { get; }
    IRepository<RelatedNews> RelatedNews { get; }
    IRepository<NewsComment> NewsComments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}