using CorporateWebsite.Web.Controllers;
using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace CorporateWebsite.Web.Controllers;

public class HomeController : Controller
{
    private readonly IPageService _pageService;
    private readonly IServiceService _serviceService;
    private readonly IProjectService _projectService;
    private readonly INewsService _newsService;
    private readonly ISliderService _sliderService;
    private readonly IBannerService _bannerService;
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;
    private readonly ISeoHelperService _seoHelper;
    private readonly ISchemaHelperService _schemaHelper;
    private readonly IBreadcrumbService _breadcrumbService;
    private readonly ISettingService _settingService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IPageService pageService,
        IServiceService serviceService,
        IProjectService projectService,
        INewsService newsService,
        ISliderService sliderService,
        IBannerService bannerService,
        ILanguageService languageService,
        ILocalizationService localizationService,
        ISeoHelperService seoHelper,
        ISchemaHelperService schemaHelper,
        IBreadcrumbService breadcrumbService,
        ISettingService settingService,
        ILogger<HomeController> logger)
    {
        _pageService = pageService;
        _serviceService = serviceService;
        _projectService = projectService;
        _newsService = newsService;
        _sliderService = sliderService;
        _bannerService = bannerService;
        _languageService = languageService;
        _localizationService = localizationService;
        _seoHelper = seoHelper;
        _schemaHelper = schemaHelper;
        _breadcrumbService = breadcrumbService;
        _settingService = settingService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var culture = _localizationService.CurrentCulture;
        
        try
        {
            // Get homepage
            var homePage = await _pageService.GetHomePageAsync(culture);
            
            // Get featured services
            var featuredServices = await _serviceService.GetFeaturedAsync(culture, 6);
            
            // Get featured projects
            var featuredProjects = await _projectService.GetFeaturedAsync(culture, 6);
            
            // Get latest news
            var latestNews = await _newsService.GetLatestAsync(culture, 5);
            
            // Get featured news
            var featuredNews = await _newsService.GetFeaturedAsync(culture, 3);
            
            // Get breaking news
            var breakingNews = await _newsService.GetBreakingAsync(culture, 2);
            
            // Get banners
            var heroBanners = await _bannerService.GetActiveBannersAsync("Hero", culture);
            var contentBanners = await _bannerService.GetActiveBannersAsync("Content", culture);

            // SEO
            var siteName = await _settingService.GetValueAsync("SiteName", "Corporate Website");
            var siteDescription = await _settingService.GetValueAsync("SiteDescription", "");
            
            _seoHelper.SetTitle($"{siteName} - {siteDescription}");
            _seoHelper.SetDescription(siteDescription);
            _seoHelper.SetOgType("website");
            
            if (homePage?.Translation != null)
            {
                _seoHelper.SetTitle(homePage.Translation.MetaTitle ?? homePage.Translation.Title);
                _seoHelper.SetDescription(homePage.Translation.MetaDescription ?? homePage.Translation.ShortDescription ?? siteDescription);
                _seoHelper.SetKeywords(homePage.Translation.MetaKeywords ?? "");
                _seoHelper.SetOgTitle(homePage.Translation.OgTitle ?? homePage.Translation.Title);
                _seoHelper.SetOgDescription(homePage.Translation.OgDescription ?? homePage.Translation.ShortDescription ?? siteDescription);
                _seoHelper.SetOgImage(homePage.Translation.OgImage ?? "");
                _seoHelper.SetCanonicalUrl($"{Request.Scheme}://{Request.Host}/{culture}/");
            }

            // Breadcrumb
            _breadcrumbService.Clear();
            _breadcrumbService.Add(Localizer["Home"], null, true);

            // Schema
            var webPageSchema = _schemaHelper.GenerateWebPageSchema(
                $"{Request.Scheme}://{Request.Host}/{culture}/",
                siteName,
                siteDescription,
                await _settingService.GetValueAsync("DefaultOgImage", "")
            );
            _seoHelper.AddSchemaJson(webPageSchema);

            var model = new HomeViewModel
            {
                HomePage = homePage,
                FeaturedServices = featuredServices,
                FeaturedProjects = featuredProjects,
                LatestNews = latestNews,
                FeaturedNews = featuredNews,
                BreakingNews = breakingNews,
                HeroBanners = heroBanners,
                ContentBanners = contentBanners,
                CurrentCulture = culture,
                IsRtl = _localizationService.IsRtl
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading homepage");
            return View("Error");
        }
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        var supportedCultures = new[] { "fa", "en", "ar" };
        if (!supportedCultures.Contains(culture))
        {
            culture = "fa";
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });

        HttpContext.Session.SetString("Culture", culture);

        return LocalRedirect(returnUrl ?? $"/{culture}/");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

public class HomeViewModel
{
    public PageDto? HomePage { get; set; }
    public IReadOnlyList<ServiceDto> FeaturedServices { get; set; } = new List<ServiceDto>();
    public IReadOnlyList<ProjectDto> FeaturedProjects { get; set; } = new List<ProjectDto>();
    public IReadOnlyList<NewsDto> LatestNews { get; set; } = new List<NewsDto>();
    public IReadOnlyList<NewsDto> FeaturedNews { get; set; } = new List<NewsDto>();
    public IReadOnlyList<NewsDto> BreakingNews { get; set; } = new List<NewsDto>();
    public IReadOnlyList<BannerDto> HeroBanners { get; set; } = new List<BannerDto>();
    public IReadOnlyList<BannerDto> ContentBanners { get; set; } = new List<BannerDto>();
    public string CurrentCulture { get; set; } = "fa";
    public bool IsRtl { get; set; }
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}