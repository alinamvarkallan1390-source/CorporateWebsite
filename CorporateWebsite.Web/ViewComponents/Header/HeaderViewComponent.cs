using CorporateWebsite.Web.ViewComponents.Header;
using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CorporateWebsite.Web.ViewComponents.Header;

public class HeaderViewComponent : ViewComponent
{
    private readonly IMenuService _menuService;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;

    public HeaderViewComponent(IMenuService menuService, ILocalizationService localizationService, ISettingService settingService)
    {
        _menuService = menuService;
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string menuName = "MainMenu", string variant = "default")
    {
        var menuItems = await _menuService.GetMenuTreeAsync(menuName);
        var logo = await _settingService.GetValueAsync("SiteLogo", "");
        var siteName = await _settingService.GetValueAsync("SiteName", "Corporate Website");
        var phone = await _settingService.GetValueAsync("CompanyPhone", "");
        var email = await _settingService.GetValueAsync("CompanyEmail", "");

        var model = new HeaderViewModel
        {
            MenuItems = menuItems,
            Logo = logo,
            SiteName = siteName,
            Phone = phone,
            Email = email,
            CurrentCulture = _localizationService.CurrentCulture,
            IsRtl = _localizationService.IsRtl,
            Variant = variant
        };

        return View(variant, model);
    }
}

public class HeaderViewModel
{
    public IReadOnlyList<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();
    public string Logo { get; set; } = "";
    public string SiteName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string CurrentCulture { get; set; } = "fa";
    public bool IsRtl { get; set; }
    public string Variant { get; set; } = "default";
}