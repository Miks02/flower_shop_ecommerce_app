using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components.Utility;

public class FooterViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View());
    }
}