using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Components;

public class FooterViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}