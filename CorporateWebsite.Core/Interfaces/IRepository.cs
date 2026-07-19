using CorporateWebsite.Core.Entities;
using System.Linq.Expressions;

namespace CorporateWebsite.Core.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy, params Expression<Func<T, object>>[] includes);
    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, params Expression<Func<T, object>>[] includes);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    IQueryable<T> Query();
    IQueryable<T> QueryNoTracking();
}

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
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}