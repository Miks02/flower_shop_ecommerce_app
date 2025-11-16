using FlowerShop.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components.Utility;

public class LoginViewComponent : ViewComponent
{

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View(new LoginViewModel()));
    }
}