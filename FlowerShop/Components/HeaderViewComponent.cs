using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components;

public class HeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}