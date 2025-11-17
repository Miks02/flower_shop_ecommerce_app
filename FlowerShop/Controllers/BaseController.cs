using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Controllers;

public class BaseController : Controller
{
    protected readonly ILogger<BaseController> _logger;

    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;          
    }

    protected void SetErrorMessage(string? message = null, Exception? ex = null)
    {
        if (ex != null && message != null)
        {
            _logger.LogError("GREŠKA!" + ex + " - " + message);
            TempData["Error"] = message;
            return;
        }

        if (ex != null)
        {
            _logger.LogError(ex, "Došlo je do greške.");
            TempData["Error"] = "Došlo je do greške, pokušajte ponovo kasnije...";
            return;
        }

        if (message != null)
        {
            _logger.LogError("GREŠKA! - {message}", message);
            TempData["Error"] = message;
            return;
        }

        _logger.LogError("GREŠKA! - Nepoznata greška");
        TempData["Error"] = "Došlo je do greške, pokušajte ponovo kasnije...";
        
       
    }
    
    protected void SetSuccessMessage(string? message = null)
    {

        if (message != null)
        {
            TempData["Success"] = message;
            return;
        }
        
        TempData["Success"] = "Operacija izvršena uspešno!";
       
    }
    
}