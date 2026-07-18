using Microsoft.AspNetCore.Identity;

namespace CorporateWebsite.Core.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int? LanguageId { get; set; }
    public string? TimeZone { get; set; }
    public bool IsActive { get; set; } = true.
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; } = 0.
    public DateTime? LockoutEndDate { get; set; }
    public bool TwoFactorEnabled { get; set; } = false.
    public string? TwoFactorSecret { get; set; }
    public string? BackupCodes { get; set; } // JSON array
    
    // Navigation
    public Language? PreferredLanguage { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public ICollection<News> AuthoredNews { get; set; } = new List<News>();
    public ICollection<FormSubmission> ProcessedSubmissions { get; set; } = new List<FormSubmission>();
}

public class ApplicationRole : IdentityRole<int>
{
    public string? Description { get; set; }
    public bool IsSystem { get; set; } = false.
    public int DisplayOrder { get; set; }
    
    // Navigation
    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}

public class ApplicationUserRole : IdentityUserRole<int>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;
}

public class RolePermission : BaseEntity
{
    public int RoleId { get; set; }
    public string Permission { get; set; } = string.Empty; // e.g., "Pages.Create", "Pages.Edit", "Users.Delete"
    public string Module { get; set; } = string.Empty; // Pages, Services, Projects, News, Users, Settings, etc.
    public string Action { get; set; } = string.Empty; // Create, Read, Update, Delete, Publish, Export
    
    // Navigation
    public ApplicationRole Role { get; set; } = null!;
}

public static class Permissions
{
    // Pages
    public const string PagesView = "Pages.View";
    public const string PagesCreate = "Pages.Create";
    public const string PagesEdit = "Pages.Edit";
    public const string PagesDelete = "Pages.Delete";
    public const string PagesPublish = "Pages.Publish";
    public const string PagesReorder = "Pages.Reorder";
    
    // Services
    public const string ServicesView = "Services.View";
    public const string ServicesCreate = "Services.Create";
    public const string ServicesEdit = "Services.Edit";
    public const string ServicesDelete = "Services.Delete";
    public const string ServicesPublish = "Services.Publish";
    public const string ServicesReorder = "Services.Reorder";
    
    // Projects
    public const string ProjectsView = "Projects.View";
    public const string ProjectsCreate = "Projects.Create";
    public const string ProjectsEdit = "Projects.Edit";
    public const string ProjectsDelete = "Projects.Delete";
    public const string ProjectsPublish = "Projects.Publish";
    public const string ProjectsReorder = "Projects.Reorder";
    
    // News
    public const string NewsView = "News.View";
    public const string NewsCreate = "News.Create";
    public const string NewsEdit = "News.Edit";
    public const string NewsDelete = "News.Delete";
    public const string NewsPublish = "News.Publish";
    public const string NewsSchedule = "News.Schedule";
    public const string NewsModerateComments = "News.ModerateComments";
    
    // Categories
    public const string CategoriesView = "Categories.View";
    public const string CategoriesCreate = "Categories.Create";
    public const string CategoriesEdit = "Categories.Edit";
    public const string CategoriesDelete = "Categories.Delete";
    
    // Media
    public const string MediaView = "Media.View";
    public const string MediaUpload = "Media.Upload";
    public const string MediaEdit = "Media.Edit";
    public const string MediaDelete = "Media.Delete";
    public const string MediaManageFolders = "Media.ManageFolders";
    
    // Menus
    public const string MenusView = "Menus.View";
    public const string MenusCreate = "Menus.Create";
    public const string MenusEdit = "Menus.Edit";
    public const string MenusDelete = "Menus.Delete";
    public const string MenusReorder = "Menus.Reorder";
    
    // Sliders/Banners
    public const string SlidersView = "Sliders.View";
    public const string SlidersCreate = "Sliders.Create";
    public const string SlidersEdit = "Sliders.Edit";
    public const string SlidersDelete = "Sliders.Delete";
    
    // Forms
    public const string FormsView = "Forms.View";
    public const string FormsCreate = "Forms.Create";
    public const string FormsEdit = "Forms.Edit";
    public const string FormsDelete = "Forms.Delete";
    public const string FormsSubmissionsView = "Forms.SubmissionsView";
    public const string FormsSubmissionsExport = "Forms.SubmissionsExport";
    public const string FormsSubmissionsManage = "Forms.SubmissionsManage";
    
    // Languages
    public const string LanguagesView = "Languages.View";
    public const string LanguagesCreate = "Languages.Create";
    public const string LanguagesEdit = "Languages.Edit";
    public const string LanguagesDelete = "Languages.Delete";
    
    // Settings
    public const string SettingsView = "Settings.View";
    public const string SettingsEdit = "Settings.Edit";
    public const string SettingsSeo = "Settings.Seo";
    public const string SettingsSecurity = "Settings.Security";
    public const string SettingsAppearance = "Settings.Appearance";
    
    // Users
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersEdit = "Users.Edit";
    public const string UsersDelete = "Users.Delete";
    public const string UsersManageRoles = "Users.ManageRoles";
    public const string UsersImpersonate = "Users.Impersonate";
    
    // Redirects
    public const string RedirectsView = "Redirects.View";
    public const string RedirectsCreate = "Redirects.Create";
    public const string RedirectsEdit = "Redirects.Edit";
    public const string RedirectsDelete = "Redirects.Delete";
    
    // Logs
    public const string LogsView = "Logs.View";
    public const string LogsClear = "Logs.Clear";
    
    // Backup
    public const string BackupCreate = "Backup.Create";
    public const string BackupRestore = "Backup.Restore";
    public const string BackupDownload = "Backup.Download";
    
    // All modules for admin
    public static readonly string[] AllModules = new[]
    {
        "Pages", "Services", "Projects", "News", "Categories", "Media",
        "Menus", "Sliders", "Forms", "Languages", "Settings", "Users",
        "Redirects", "Logs", "Backup"
    };
    
    public static readonly string[] AllActions = new[]
    {
        "View", "Create", "Edit", "Delete", "Publish", "Reorder",
        "Schedule", "ModerateComments", "Upload", "ManageFolders",
        "SubmissionsView", "SubmissionsExport", "SubmissionsManage",
        "ManageRoles", "Impersonate", "Seo", "Security", "Appearance",
        "Create", "Restore", "Download", "Clear"
    };
}