using CorporateWebsite.Web.ViewComponents.Footer;
using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CorporateWebsite.Web.ViewComponents.Footer;

public class FooterViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;
    private readonly ISettingService _settingService;
    private readonly ILocalizationService _localizationService;

    public FooterViewComponent(IMenuService menuService, ISettingService settingService, ILocalizationService localizationService)
    {
        _menuService = menuService;
        _settingService = settingService;
        _localizationService = localizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string variant = "default")
    {
        var footerMenu = await _menuService.GetMenuTreeAsync("FooterMenu");
        var socialLinks = await GetSocialLinksAsync();
        var siteName = await _settingService.GetValueAsync("SiteName", "Corporate Website");
        var companyName = await _settingService.GetValueAsync("CompanyName", siteName);
        var address = await _settingService.GetValueAsync("CompanyAddress", "");
        var phone = await _settingService.GetValueAsync("CompanyPhone", "");
        var email = await _settingService.GetValueAsync("CompanyEmail", "");
        var workingHours = await _settingService.GetValueAsync("CompanyWorkingHours", "");
        var description = await _settingService.GetValueAsync("SiteDescription", "");
        var logo = await _settingService.GetValueAsync("SiteLogo", "");

        var model = new FooterViewModel
        {
            FooterMenuItems = footerMenu,
            SocialLinks = socialLinks,
            SiteName = siteName,
            CompanyName = companyName,
            Address = address,
            Phone = phone,
            Email = email,
            WorkingHours = workingHours,
            Description = description,
            Logo = logo,
            CurrentCulture = _localizationService.CurrentCulture,
            IsRtl = _localizationService.IsRtl,
            Variant = variant,
            CurrentYear = DateTime.Now.Year
        };

        return View(variant, model);
    }

    private async Task<List<SocialLinkDto>> GetSocialLinksAsync()
    {
        var links = new List<SocialLinkDto>();
        
        var facebook = await _settingService.GetValueAsync("FacebookUrl", "");
        if (!string.IsNullOrEmpty(facebook))
            links.Add(new SocialLinkDto { Name = "Facebook", Url = facebook, Icon = "fab fa-facebook-f", Color = "#1877F2" });

        var twitter = await _settingService.GetValueAsync("TwitterUrl", "");
        if (!string.IsNullOrEmpty(twitter))
            links.Add(new SocialLinkDto { Name = "Twitter", Url = twitter, Icon = "fab fa-twitter", Color = "#1DA1F2" });

        var instagram = await _settingService.GetValueAsync("InstagramUrl", "");
        if (!string.IsNullOrEmpty(instagram))
            links.Add(new SocialLinkDto { Name = "Instagram", Url = instagram, Icon = "fab fa-instagram", Color = "#E4405F" });

        var linkedin = await _settingService.GetValueAsync("LinkedInUrl", "");
        if (!string.IsNullOrEmpty(linkedin))
            links.Add(new SocialLinkDto { Name = "LinkedIn", Url = linkedin, Icon = "fab fa-linkedin-in", Color = "#0A66C2" });

        var youtube = await _settingService.GetValueAsync("YouTubeUrl", "");
        if (!string.IsNullOrEmpty(youtube))
            links.Add(new SocialLinkDto { Name = "YouTube", Url = youtube, Icon = "fab fa-youtube", Color = "#FF0000" });

        var telegram = await _settingService.GetValueAsync("TelegramUrl", "");
        if (!string.IsNullOrEmpty(telegram))
            links.Add(new SocialLinkDto { Name = "Telegram", Url = telegram, Icon = "fab fa-telegram-plane", Color = "#0088CC" });

        var whatsapp = await _settingService.GetValueAsync("WhatsAppNumber", "");
        if (!string.IsNullOrEmpty(whatsapp))
            links.Add(new SocialLinkDto { Name = "WhatsApp", Url = $"https://wa.me/{whatsapp}", Icon = "fab fa-whatsapp", Color = "#25D366" });

        return links;
    }
}

public class FooterViewModel
{
    public IReadOnlyList<MenuItemDto> FooterMenuItems { get; set; } = new List<MenuItemDto>();
    public List<SocialLinkDto> SocialLinks { get; set; } = new();
    public string SiteName { get; set; } = "";
    public string CompanyName { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string WorkingHours { get; set; } = "";
    public string Description { get; set; } = "";
    public string Logo { get; set; } = "";
    public string CurrentCulture { get; set; } = "fa";
    public bool IsRtl { get; set; }
    public string Variant { get; set; } = "default";
    public int CurrentYear { get; set; }
}

public class SocialLinkDto
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Color { get; set; } = "";
}