using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CorporateWebsite.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    private IRepository<Language>? _languages;
    private IRepository<Page>? _pages;
    private IRepository<PageTranslation>? _pageTranslations;
    private IRepository<Service>? _services;
    private IRepository<ServiceTranslation>? _serviceTranslations;
    private IRepository<ServiceCategory>? _serviceCategories;
    private IRepository<ServiceCategoryTranslation>? _serviceCategoryTranslations;
    private IRepository<Project>? _projects;
    private IRepository<ProjectTranslation>? _projectTranslations;
    private IRepository<ProjectCategory>? _projectCategories;
    private IRepository<ProjectCategoryTranslation>? _projectCategoryTranslations;
    private IRepository<News>? _news;
    private IRepository<NewsTranslation>? _newsTranslations;
    private IRepository<NewsCategory>? _newsCategories;
    private IRepository<NewsCategoryTranslation>? _newsCategoryTranslations;
    private IRepository<Tag>? _tags;
    private IRepository<TagTranslation>? _tagTranslations;
    private IRepository<Menu>? _menus;
    private IRepository<MenuTranslation>? _menuTranslations;
    private IRepository<MenuItem>? _menuItems;
    private IRepository<MenuItemTranslation>? _menuItemTranslations;
    private IRepository<Setting>? _settings;
    private IRepository<SettingTranslation>? _settingTranslations;
    private IRepository<Form>? _forms;
    private IRepository<FormTranslation>? _formTranslations;
    private IRepository<FormField>? _formFields;
    private IRepository<FormFieldTranslation>? _formFieldTranslations;
    private IRepository<FormSubmission>? _formSubmissions;
    private IRepository<FormFieldValue>? _formFieldValues;
    private IRepository<FormSubmissionFile>? _formSubmissionFiles;
    private IRepository<Slider>? _sliders;
    private IRepository<SliderTranslation>? _sliderTranslations;
    private IRepository<SliderItem>? _sliderItems;
    private IRepository<SliderItemTranslation>? _sliderItemTranslations;
    private IRepository<Banner>? _banners;
    private IRepository<BannerTranslation>? _bannerTranslations;
    private IRepository<MediaFile>? _mediaFiles;
    private IRepository<MediaFileTranslation>? _mediaFileTranslations;
    private IRepository<MediaFolder>? _mediaFolders;
    private IRepository<MediaFolderTranslation>? _mediaFolderTranslations;
    private IRepository<Redirect>? _redirects;
    private IRepository<ActivityLog>? _activityLogs;
    private IRepository<BrokenLink>? _brokenLinks;
    private IRepository<ScheduledTask>? _scheduledTasks;
    private IRepository<ScheduledTaskLog>? _scheduledTaskLogs;
    private IRepository<EmailTemplate>? _emailTemplates;
    private IRepository<EmailTemplateTranslation>? _emailTemplateTranslations;
    private IRepository<EmailQueue>? _emailQueue;
    private IRepository<Backup>? _backups;
    private IRepository<RolePermission>? _rolePermissions;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<Language> Languages => _languages ??= new Repository<Language>(_context);
    public IRepository<Page> Pages => _pages ??= new Repository<Page>(_context);
    public IRepository<PageTranslation> PageTranslations => _pageTranslations ??= new Repository<PageTranslation>(_context);
    public IRepository<Service> Services => _services ??= new Repository<Service>(_context);
    public IRepository<ServiceTranslation> ServiceTranslations => _serviceTranslations ??= new Repository<ServiceTranslation>(_context);
    public IRepository<ServiceCategory> ServiceCategories => _serviceCategories ??= new Repository<ServiceCategory>(_context);
    public IRepository<ServiceCategoryTranslation> ServiceCategoryTranslations => _serviceCategoryTranslations ??= new Repository<ServiceCategoryTranslation>(_context);
    public IRepository<Project> Projects => _projects ??= new Repository<Project>(_context);
    public IRepository<ProjectTranslation> ProjectTranslations => _projectTranslations ??= new Repository<ProjectTranslation>(_context);
    public IRepository<ProjectCategory> ProjectCategories => _projectCategories ??= new Repository<ProjectCategory>(_context);
    public IRepository<ProjectCategoryTranslation> ProjectCategoryTranslations => _projectCategoryTranslations ??= new Repository<ProjectCategoryTranslation>(_context);
    public IRepository<News> News => _news ??= new Repository<News>(_context);
    public IRepository<NewsTranslation> NewsTranslations => _newsTranslations ??= new Repository<NewsTranslation>(_context);
    public IRepository<NewsCategory> NewsCategories => _newsCategories ??= new Repository<NewsCategory>(_context);
    public IRepository<NewsCategoryTranslation> NewsCategoryTranslations => _newsCategoryTranslations ??= new Repository<NewsCategoryTranslation>(_context);
    public IRepository<Tag> Tags => _tags ??= new Repository<Tag>(_context);
    public IRepository<TagTranslation> TagTranslations => _tagTranslations ??= new Repository<TagTranslation>(_context);
    public IRepository<Menu> Menus => _menus ??= new Repository<Menu>(_context);
    public IRepository<MenuTranslation> MenuTranslations => _menuTranslations ??= new Repository<MenuTranslation>(_context);
    public IRepository<MenuItem> MenuItems => _menuItems ??= new Repository<MenuItem>(_context);
    public IRepository<MenuItemTranslation> MenuItemTranslations => _menuItemTranslations ??= new Repository<MenuItemTranslation>(_context);
    public IRepository<Setting> Settings => _settings ??= new Repository<Setting>(_context);
    public IRepository<SettingTranslation> SettingTranslations => _settingTranslations ??= new Repository<SettingTranslation>(_context);
    public IRepository<Form> Forms => _forms ??= new Repository<Form>(_context);
    public IRepository<FormTranslation> FormTranslations => _formTranslations ??= new Repository<FormTranslation>(_context);
    public IRepository<FormField> FormFields => _formFields ??= new Repository<FormField>(_context);
    public IRepository<FormFieldTranslation> FormFieldTranslations => _formFieldTranslations ??= new Repository<FormFieldTranslation>(_context);
    public IRepository<FormSubmission> FormSubmissions => _formSubmissions ??= new Repository<FormSubmission>(_context);
    public IRepository<FormFieldValue> FormFieldValues => _formFieldValues ??= new Repository<FormFieldValue>(_context);
    public IRepository<FormSubmissionFile> FormSubmissionFiles => _formSubmissionFiles ??= new Repository<FormSubmissionFile>(_context);
    public IRepository<Slider> Sliders => _sliders ??= new Repository<Slider>(_context);
    public IRepository<SliderTranslation> SliderTranslations => _sliderTranslations ??= new Repository<SliderTranslation>(_context);
    public IRepository<SliderItem> SliderItems => _sliderItems ??= new Repository<SliderItem>(_context);
    public IRepository<SliderItemTranslation> SliderItemTranslations => _sliderItemTranslations ??= new Repository<SliderItemTranslation>(_context);
    public IRepository<Banner> Banners => _banners ??= new Repository<Banner>(_context);
    public IRepository<BannerTranslation> BannerTranslations => _bannerTranslations ??= new Repository<BannerTranslation>(_context);
    public IRepository<MediaFile> MediaFiles => _mediaFiles ??= new Repository<MediaFile>(_context);
    public IRepository<MediaFileTranslation> MediaFileTranslations => _mediaFileTranslations ??= new Repository<MediaFileTranslation>(_context);
    public IRepository<MediaFolder> MediaFolders => _mediaFolders ??= new Repository<MediaFolder>(_context);
    public IRepository<MediaFolderTranslation> MediaFolderTranslations => _mediaFolderTranslations ??= new Repository<MediaFolderTranslation>(_context);
    public IRepository<Redirect> Redirects => _redirects ??= new Repository<Redirect>(_context);
    public IRepository<ActivityLog> ActivityLogs => _activityLogs ??= new Repository<ActivityLog>(_context);
    public IRepository<BrokenLink> BrokenLinks => _brokenLinks ??= new Repository<BrokenLink>(_context);
    public IRepository<ScheduledTask> ScheduledTasks => _scheduledTasks ??= new Repository<ScheduledTask>(_context);
    public IRepository<ScheduledTaskLog> ScheduledTaskLogs => _scheduledTaskLogs ??= new Repository<ScheduledTaskLog>(_context);
    public IRepository<EmailTemplate> EmailTemplates => _emailTemplates ??= new Repository<EmailTemplate>(_context);
    public IRepository<EmailTemplateTranslation> EmailTemplateTranslations => _emailTemplateTranslations ??= new Repository<EmailTemplateTranslation>(_context);
    public IRepository<EmailQueue> EmailQueue => _emailQueue ??= new Repository<EmailQueue>(_context);
    public IRepository<Backup> Backups => _backups ??= new Repository<Backup>(_context);
    public IRepository<RolePermission> RolePermissions => _rolePermissions ??= new Repository<RolePermission>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
    }
}