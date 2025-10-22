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
        bool isHome = false)
    {

        var vm = new HeroViewModel
        {
            Title = title,
            Subtitle = subtitle,
            BackgroundImage = backgroundImage ?? "'./AppImages/Hero pozadina.jpg'",
            IsHome = isHome
        };
        
        return await Task.FromResult(View(vm));
    }
}