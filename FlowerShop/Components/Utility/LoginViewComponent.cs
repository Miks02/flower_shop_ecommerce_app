using FlowerShop.Web.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Components.Utility;

public class LoginViewComponent : ViewComponent
{

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View(new LoginViewModel()));
    }
}