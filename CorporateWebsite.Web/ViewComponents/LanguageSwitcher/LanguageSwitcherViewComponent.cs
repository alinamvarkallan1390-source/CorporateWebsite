using CorporateWebsite.Web.ViewComponents.LanguageSwitcher;
using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CorporateWebsite.Web.ViewComponents.LanguageSwitcher;

public class LanguageSwitcherViewComponent : ViewComponent
{
    private readonly ILanguageService _languageService;
    private readonly ILocalizationService _localizationService;

    public LanguageSwitcherViewComponent(ILanguageService languageService, ILocalizationService localizationService)
    {
        _languageService = languageService;
        _localizationService = localizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string variant = "dropdown")
    {
        var languages = await _languageService.GetAllActiveAsync();
        var currentCulture = _localizationService.CurrentCulture;

        var model = new LanguageSwitcherViewModel
        {
            Languages = languages,
            CurrentCulture = currentCulture,
            Variant = variant
        };

        return View(variant, model);
    }
}

public class LanguageSwitcherViewModel
{
    public IReadOnlyList<LanguageDto> Languages { get; set; } = new List<LanguageDto>();
    public string CurrentCulture { get; set; } = "fa";
    public string Variant { get; set; } = "dropdown";
}