using CorporateWebsite.Core.Entities;
using CorporateWebsite.Core.DTOs;

namespace CorporateWebsite.Application.Interfaces;

public interface IMenuService
{
    Task<MenuDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<MenuDto?> GetByNameAsync(string name, string languageCode);
    Task<IReadOnlyList<MenuDto>> GetAllAsync(string? languageCode = null);
    Task<MenuDto> CreateAsync(CreateMenuDto dto);
    Task<MenuDto> UpdateAsync(int id, UpdateMenuDto dto);
    Task DeleteAsync(int id);
    Task<IReadOnlyList<MenuItemDto>> GetMenuItemsAsync(string menuName, string languageCode);
    Task<MenuItemDto> CreateMenuItemAsync(CreateMenuItemDto dto);
    Task<MenuItemDto> UpdateMenuItemAsync(int id, UpdateMenuItemDto dto);
    Task DeleteMenuItemAsync(int id);
    Task ReorderMenuItemsAsync(int menuId, List<ReorderItemDto> items);
    Task<IReadOnlyList<MenuItemDto>> GetMenuTreeAsync(string menuName, string languageCode);
}

public interface ISettingService
{
    Task<SettingDto?> GetByKeyAsync(string key, string? languageCode = null);
    Task<T?> GetValueAsync<T>(string key, T? defaultValue = default);
    Task<string?> GetValueAsync(string key, string? defaultValue = null);
    Task<SettingDto> SetValueAsync(string key, string value, string group = "General", string dataType = "String", bool isPublic = false, bool isEncrypted = false);
    Task<IReadOnlyList<SettingDto>> GetAllAsync(string? group = null, string? languageCode = null);
    Task<IReadOnlyList<SettingDto>> GetPublicSettingsAsync(string languageCode);
    Task<Dictionary<string, string>> GetSettingsDictionaryAsync(string? group = null, string? languageCode = null);
    Task<bool> DeleteAsync(string key);
    Task<SettingDto> UpdateAsync(int id, UpdateSettingDto dto);
    Task InitializeDefaultSettingsAsync();
}

public interface IFormService
{
    Task<FormDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<FormDto?> GetBySlugAsync(string slug, string languageCode);
    Task<FormDto?> GetByPageAsync(int pageId, string languageCode);
    Task<IReadOnlyList<FormDto>> GetAllAsync(string? languageCode = null, bool? isActive = null);
    Task<FormDto> CreateAsync(CreateFormDto dto);
    Task<FormDto> UpdateAsync(int id, UpdateFormDto dto);
    Task DeleteAsync(int id);
    Task<FormSubmissionDto> SubmitAsync(SubmitFormDto dto, string ipAddress, string userAgent, string referrerUrl, int languageId);
    Task<FormSubmissionDto?> GetSubmissionByIdAsync(int id);
    Task<(IReadOnlyList<FormSubmissionDto> Items, int TotalCount)> GetSubmissionsPagedAsync(FormSubmissionFilterDto filter);
    Task UpdateSubmissionStatusAsync(int id, string status, string? notes = null, string? processedBy = null);
    Task<byte[]> ExportSubmissionsToExcelAsync(int formId, FormSubmissionFilterDto filter);
    Task<byte[]> ExportSubmissionsToCsvAsync(int formId, FormSubmissionFilterDto filter);
    Task<bool> ValidateSubmissionAsync(SubmitFormDto dto, int formId);
    Task SendNotificationEmailsAsync(FormSubmissionDto submission);
    Task SendConfirmationEmailAsync(FormSubmissionDto submission);
}

public interface ISliderService
{
    Task<SliderDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<SliderDto?> GetByNameAsync(string name, string languageCode);
    Task<IReadOnlyList<SliderDto>> GetAllAsync(string? languageCode = null, bool? isActive = null, string? location = null);
    Task<SliderDto> CreateAsync(CreateSliderDto dto);
    Task<SliderDto> UpdateAsync(int id, UpdateSliderDto dto);
    Task DeleteAsync(int id);
    Task<SliderItemDto> CreateSliderItemAsync(CreateSliderItemDto dto);
    Task<SliderItemDto> UpdateSliderItemAsync(int id, UpdateSliderItemDto dto);
    Task DeleteSliderItemAsync(int id);
    Task ReorderItemsAsync(int sliderId, List<ReorderItemDto> items);
    Task<IReadOnlyList<SliderItemDto>> GetActiveItemsAsync(int sliderId, string languageCode);
}

public interface IBannerService
{
    Task<BannerDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<IReadOnlyList<BannerDto>> GetAllAsync(string? languageCode = null, bool? isActive = null, string? location = null);
    Task<BannerDto> CreateAsync(CreateBannerDto dto);
    Task<BannerDto> UpdateAsync(int id, UpdateBannerDto dto);
    Task DeleteAsync(int id);
    Task<IReadOnlyList<BannerDto>> GetActiveBannersAsync(string location, string languageCode, DateTime? currentDate = null);
}

public interface IMediaService
{
    Task<MediaFileDto?> GetByIdAsync(int id, string? languageCode = null);
    Task<IReadOnlyList<MediaFileDto>> GetAllAsync(string? folder = null, string? search = null, string? fileType = null);
    Task<(IReadOnlyList<MediaFileDto> Items, int TotalCount)> GetPagedAsync(MediaFilterDto filter);
    Task<MediaFileDto> UploadAsync(UploadMediaDto dto, Stream fileStream, string uploadedBy);
    Task<MediaFileDto> UpdateAsync(int id, UpdateMediaDto dto);
    Task DeleteAsync(int id);
    Task DeleteRangeAsync(IEnumerable<int> ids);
    Task<MediaFolderDto?> GetFolderByIdAsync(int id, string? languageCode = null);
    Task<IReadOnlyList<MediaFolderDto>> GetFoldersAsync(int? parentId = null, string? languageCode = null);
    Task<MediaFolderDto> CreateFolderAsync(CreateMediaFolderDto dto);
    Task<MediaFolderDto> UpdateFolderAsync(int id, UpdateMediaFolderDto dto);
    Task DeleteFolderAsync(int id);
    Task<MediaFileDto?> GetByEntityAsync(string entityType, int entityId, string? languageCode = null);
    Task<string> GetOptimizedImageUrlAsync(int mediaId, int width, int height, string format = "webp", int quality = 80);
    Task<byte[]> GetFileBytesAsync(int id);
    Task<Stream> GetFileStreamAsync(int id);
}

public interface IRedirectService
{
    Task<RedirectDto?> GetByIdAsync(int id);
    Task<RedirectDto?> GetBySourceUrlAsync(string sourceUrl, int? languageId = null);
    Task<IReadOnlyList<RedirectDto>> GetAllAsync(bool? isActive = null);
    Task<(IReadOnlyList<RedirectDto> Items, int TotalCount)> GetPagedAsync(RedirectFilterDto filter);
    Task<RedirectDto> CreateAsync(CreateRedirectDto dto);
    Task<RedirectDto> UpdateAsync(int id, UpdateRedirectDto dto);
    Task DeleteAsync(int id);
    Task ProcessRedirectAsync(string sourceUrl, int? languageId);
    Task<IReadOnlyList<RedirectDto>> GetBrokenLinksAsync();
    Task FixBrokenLinkAsync(int id, string fixedUrl);
}

public interface ISeoService
{
    Task<SeoDataDto> GetPageSeoAsync(string pageType, int entityId, string languageCode);
    Task<string> GenerateSitemapAsync();
    Task<string> GenerateRobotsTxtAsync();
    Task<Dictionary<string, string>> GetHreflangUrlsAsync(string pageType, int entityId);
    Task<string> GenerateSchemaJsonAsync(string schemaType, object data);
    Task<bool> ValidateSeoAsync(SeoValidationDto dto);
    Task<SeoAuditDto> AuditPageAsync(string url);
    Task SubmitToSearchConsoleAsync(string url);
    Task<IReadOnlyList<SearchConsoleErrorDto>> GetSearchConsoleErrorsAsync();
}

public interface IActivityLogService
{
    Task LogAsync(string userId, string userName, string action, string module, int? entityId = null, string? entityType = null, string? entityName = null, object? oldValues = null, object? newValues = null, string? ipAddress = null, string? userAgent = null, string? url = null, string? method = null, bool isSuccess = true, string? errorMessage = null, string? correlationId = null);
    Task<(IReadOnlyList<ActivityLogDto> Items, int TotalCount)> GetPagedAsync(ActivityLogFilterDto filter);
    Task<IReadOnlyList<ActivityLogDto>> GetByEntityAsync(string entityType, int entityId);
    Task<IReadOnlyList<ActivityLogDto>> GetByUserAsync(string userId, int count = 50);
    Task ClearOldLogsAsync(int olderThanDays = 90);
}

public interface IBackupService
{
    Task<BackupDto> CreateBackupAsync(string name, string type = "Full", string? initiatedBy = null, bool isAutomatic = false);
    Task<BackupDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<BackupDto>> GetAllAsync();
    Task<bool> RestoreAsync(int id);
    Task DeleteAsync(int id);
    Task<Stream> DownloadAsync(int id);
    Task CleanupOldBackupsAsync(int retentionDays = 30);
    Task<BackupStatusDto> GetStatusAsync();
}

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string bodyHtml, string? bodyText = null, string? fromEmail = null, string? fromName = null, string? cc = null, string? bcc = null, IEnumerable<string>? attachments = null);
    Task SendTemplateAsync(string templateName, string toEmail, Dictionary<string, string> variables, string languageCode = "fa", string? fromEmail = null, string? fromName = null);
    Task QueueEmailAsync(EmailQueueDto dto);
    Task ProcessQueueAsync(int batchSize = 50);
    Task<IReadOnlyList<EmailQueueDto>> GetFailedEmailsAsync(int maxRetries = 3);
    Task RetryFailedEmailsAsync();
}

public interface IScheduledTaskService
{
    Task<ScheduledTaskDto?> GetByIdAsync(int id);
    Task<IReadOnlyList<ScheduledTaskDto>> GetAllAsync(bool? isActive = null);
    Task<ScheduledTaskDto> CreateAsync(CreateScheduledTaskDto dto);
    Task<ScheduledTaskDto> UpdateAsync(int id, UpdateScheduledTaskDto dto);
    Task DeleteAsync(int id);
    Task RunTaskAsync(int id);
    Task ProcessDueTasksAsync();
    Task<ScheduledTaskLogDto?> GetLastLogAsync(int taskId);
}

public interface ISearchService
{
    Task<SearchResultDto> SearchAsync(string query, string languageCode, int page = 1, int pageSize = 10, string[]? types = null);
    Task ReindexAsync();
    Task IndexEntityAsync(string entityType, int entityId);
    Task RemoveFromIndexAsync(string entityType, int entityId);
    Task<IReadOnlyList<SearchSuggestionDto>> GetSuggestionsAsync(string query, string languageCode, int count = 10);
}