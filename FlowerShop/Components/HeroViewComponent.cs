using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components;

public class HeroViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync
    (
        string title, 
        string? subtitle = null, 
        string? backgroundImage = null, 
        bool isHome = false,
        bool isFullScreen = false)
    {

        string defaultImage = Url.Content("~/AppImages/Hero pozadina.jpg");
        backgroundImage = Url.Content(backgroundImage);

        var vm = new HeroViewModel
        {
            Title = title,
            Subtitle = subtitle,
            BackgroundImage = backgroundImage ?? defaultImage,
            IsHome = isHome,
            IsFullScreen = isFullScreen
        };
        
        return await Task.FromResult(View(vm));
    }
}