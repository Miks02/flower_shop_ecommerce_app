using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components;

public class HeaderViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return await Task.FromResult(View());
    }
}