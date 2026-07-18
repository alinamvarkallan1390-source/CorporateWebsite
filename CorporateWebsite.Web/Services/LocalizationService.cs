using CorporateWebsite.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace CorporateWebsite.Web.Services;

public interface ILocalizationService
{
    string CurrentCulture { get; }
    string CurrentUICulture { get; }
    bool IsRtl { get; }
    CultureInfo CultureInfo { get; }
    IEnumerable<CultureInfo> SupportedCultures { get; }
    string GetLocalizedUrl(string path, string culture);
    Task SetCultureAsync(string culture, string returnUrl);
    string GetLanguageDirection();
    string GetLanguageCode();
    string GetLanguageNativeName();
}

public class LocalizationService : ILocalizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IStringLocalizer _localizer;
    private readonly string[] _supportedCultures = { "fa", "en", "ar" };
    private readonly Dictionary<string, (string Culture, string NativeName, bool IsRtl)> _cultureInfo = new()
    {
        { "fa", ("fa-IR", "فارسی", true) },
        { "en", ("en-US", "English", false) },
        { "ar", ("ar-SA", "العربية", true) }
    };

    public LocalizationService(IHttpContextAccessor httpContextAccessor, IStringLocalizerFactory factory)
    {
        _httpContextAccessor = httpContextAccessor;
        _localizer = factory.Create(typeof(LocalizationService));
    }

    public string CurrentCulture
    {
        get
        {
            var culture = _httpContextAccessor.HttpContext?.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>()?.RequestCulture.Culture.TwoLetterISOLanguageName;
            return culture ?? "fa";
        }
    }

    public string CurrentUICulture => CurrentCulture;

    public bool IsRtl => _cultureInfo.TryGetValue(CurrentCulture, out var info) && info.IsRtl;

    public CultureInfo CultureInfo => new CultureInfo(_cultureInfo.TryGetValue(CurrentCulture, out var info) ? info.Culture : "fa-IR");

    public IEnumerable<CultureInfo> SupportedCultures => _supportedCultures.Select(c => new CultureInfo(_cultureInfo[c].Culture));

    public string GetLocalizedUrl(string path, string culture)
    {
        if (string.IsNullOrEmpty(path)) path = "/";
        
        // Remove existing culture prefix
        var cleanPath = path;
        foreach (var c in _supportedCultures)
        {
            if (cleanPath.StartsWith($"/{c}/", StringComparison.OrdinalIgnoreCase) || cleanPath == $"/{c}")
            {
                cleanPath = cleanPath.Substring(c.Length + 1);
                if (string.IsNullOrEmpty(cleanPath)) cleanPath = "/";
                break;
            }
        }

        // Add new culture prefix
        return $"/{culture}{cleanPath}";
    }

    public async Task SetCultureAsync(string culture, string returnUrl)
    {
        if (!_supportedCultures.Contains(culture))
        {
            culture = "fa";
        }

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            // Set cookie
            context.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });

            // Set session
            context.Session.SetString("Culture", culture);
        }
    }

    public string GetLanguageDirection() => IsRtl ? "rtl" : "ltr";

    public string GetLanguageCode() => CurrentCulture;

    public string GetLanguageNativeName() => _cultureInfo.TryGetValue(CurrentCulture, out var info) ? info.NativeName : "فارسی";
}