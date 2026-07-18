using CorporateWebsite.Web.ViewComponents.Slider;
using CorporateWebsite.Application.Interfaces;
using CorporateWebsite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CorporateWebsite.Web.ViewComponents.Slider;

public class SliderViewComponent : ViewComponent
{
    private readonly ISliderService _sliderService;
    private readonly ILocalizationService _localizationService;

    public SliderViewComponent(ISliderService sliderService, ILocalizationService localizationService)
    {
        _sliderService = sliderService;
        _localizationService = localizationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string sliderName = "Homepage", string variant = "default")
    {
        var slider = await _sliderService.GetByNameAsync(sliderName, _localizationService.CurrentCulture);
        
        if (slider == null)
        {
            return Content("");
        }

        var items = await _sliderService.GetActiveItemsAsync(slider.Id, _localizationService.CurrentCulture);

        var model = new SliderViewModel
        {
            Slider = slider,
            Items = items,
            CurrentCulture = _localizationService.CurrentCulture,
            IsRtl = _localizationService.IsRtl,
            Variant = variant
        };

        return View(variant, model);
    }
}

public class SliderViewModel
{
    public SliderDto? Slider { get; set; }
    public IReadOnlyList<SliderItemDto> Items { get; set; } = new List<SliderItemDto>();
    public string CurrentCulture { get; set; } = "fa";
    public bool IsRtl { get; set; }
    public string Variant { get; set; } = "default";
}