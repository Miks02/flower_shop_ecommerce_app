using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Web.Components.Utility;

public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View());
    }
}