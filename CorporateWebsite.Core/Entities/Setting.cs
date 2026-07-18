namespace CorporateWebsite.Core.Entities;

public class Setting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Group { get; set; } = "General"; // General, SEO, Social, Email, Security, Appearance, etc.
    public string DataType { get; set; } = "String"; // String, Integer, Boolean, JSON, Image, Color, HTML
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false; // Can be accessed from frontend
    public bool IsEncrypted { get; set; } = false; // For sensitive data like API keys
    public int DisplayOrder { get; set; }
    
    // Navigation
    public ICollection<SettingTranslation> Translations { get; set; } = new List<SettingTranslation>();
}

public class SettingTranslation : BaseEntity
{
    public int SettingId { get; set; }
    public int LanguageId { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
    
    // Navigation
    public Setting Setting { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

// Common Setting Keys
public static class SettingKeys
{
    // General
    public const string SiteName = "SiteName";
    public const string SiteDescription = "SiteDescription";
    public const string SiteKeywords = "SiteKeywords";
    public const string SiteLogo = "SiteLogo";
    public const string SiteFavicon = "SiteFavicon";
    public const string DefaultLanguage = "DefaultLanguage";
    public const string SupportedLanguages = "SupportedLanguages";
    public const string TimeZone = "TimeZone";
    public const string DateFormat = "DateFormat";
    public const string MaintenanceMode = "MaintenanceMode";
    public const string MaintenanceMessage = "MaintenanceMessage";
    
    // SEO
    public const string DefaultMetaTitle = "DefaultMetaTitle";
    public const string DefaultMetaDescription = "DefaultMetaDescription";
    public const string DefaultMetaKeywords = "DefaultMetaKeywords";
    public const string DefaultOgImage = "DefaultOgImage";
    public const string GoogleSiteVerification = "GoogleSiteVerification";
    public const string BingSiteVerification = "BingSiteVerification";
    public const string YandexSiteVerification = "YandexSiteVerification";
    public const string RobotsTxt = "RobotsTxt";
    public const string SitemapEnabled = "SitemapEnabled";
    public const string CanonicalDomain = "CanonicalDomain";
    
    // Schema
    public const string OrganizationSchema = "OrganizationSchema";
    public const string WebsiteSchema = "WebsiteSchema";
    
    // Social
    public const string FacebookUrl = "FacebookUrl";
    public const string TwitterUrl = "TwitterUrl";
    public const string InstagramUrl = "InstagramUrl";
    public const string LinkedInUrl = "LinkedInUrl";
    public const string YouTubeUrl = "YouTubeUrl";
    public const string TelegramUrl = "TelegramUrl";
    public const string WhatsAppNumber = "WhatsAppNumber";
    public const string SocialShareEnabled = "SocialShareEnabled";
    
    // Contact
    public const string CompanyName = "CompanyName";
    public const string CompanyAddress = "CompanyAddress";
    public const string CompanyPhone = "CompanyPhone";
    public const string CompanyEmail = "CompanyEmail";
    public const string CompanyWorkingHours = "CompanyWorkingHours";
    public const string GoogleMapsApiKey = "GoogleMapsApiKey";
    public const string GoogleMapsEmbedUrl = "GoogleMapsEmbedUrl";
    public const string Latitude = "Latitude";
    public const string Longitude = "Longitude";
    
    // Email
    public const string SmtpHost = "SmtpHost";
    public const string SmtpPort = "SmtpPort";
    public const string SmtpUsername = "SmtpUsername";
    public const string SmtpPassword = "SmtpPassword";
    public const string SmtpEnableSsl = "SmtpEnableSsl";
    public const string SmtpFromEmail = "SmtpFromEmail";
    public const string SmtpFromName = "SmtpFromName";
    public const string AdminNotificationEmails = "AdminNotificationEmails";
    
    // Security
    public const string RecaptchaSiteKey = "RecaptchaSiteKey";
    public const string RecaptchaSecretKey = "RecaptchaSecretKey";
    public const string RecaptchaVersion = "RecaptchaVersion"; // v2, v3
    public const string RateLimitRequests = "RateLimitRequests";
    public const string RateLimitWindowMinutes = "RateLimitWindowMinutes";
    public const string EnableTwoFactor = "EnableTwoFactor";
    public const string PasswordMinLength = "PasswordMinLength";
    public const string PasswordRequireDigit = "PasswordRequireDigit";
    public const string PasswordRequireUppercase = "PasswordRequireUppercase";
    public const string PasswordRequireSpecialChar = "PasswordRequireSpecialChar";
    public const string LockoutMaxFailedAttempts = "LockoutMaxFailedAttempts";
    public const string LockoutDurationMinutes = "LockoutDurationMinutes";
    
    // Analytics
    public const string GoogleAnalyticsId = "GoogleAnalyticsId";
    public const string GoogleTagManagerId = "GoogleTagManagerId";
    public const string FacebookPixelId = "FacebookPixelId";
    public const string CustomHeadScripts = "CustomHeadScripts";
    public const string CustomBodyScripts = "CustomBodyScripts";
    
    // File Upload
    public const string MaxFileSizeMb = "MaxFileSizeMb";
    public const string AllowedImageTypes = "AllowedImageTypes";
    public const string AllowedDocumentTypes = "AllowedDocumentTypes";
    public const string AllowedVideoTypes = "AllowedVideoTypes";
    public const string UploadPath = "UploadPath";
    public const string UseCdn = "UseCdn";
    public const string CdnUrl = "CdnUrl";
    
    // Performance
    public const string EnableCompression = "EnableCompression";
    public const string EnableCaching = "EnableCaching";
    public const string CacheDurationMinutes = "CacheDurationMinutes";
    public const string EnableMinification = "EnableMinification";
    public const string EnableImageOptimization = "EnableImageOptimization";
    public const string ImageQuality = "ImageQuality";
    public const string WebPEnabled = "WebPEnabled";
    public const string AvifEnabled = "AvifEnabled";
    
    // Appearance
    public const string PrimaryColor = "PrimaryColor";
    public const string SecondaryColor = "SecondaryColor";
    public const string AccentColor = "AccentColor";
    public const string FontFamily = "FontFamily";
    public const string FontFamilyRtl = "FontFamilyRtl";
    public const string HeaderStyle = "HeaderStyle";
    public const string FooterStyle = "FooterStyle";
    public const string CustomCss = "CustomCss";
    public const string DarkModeEnabled = "DarkModeEnabled";
}