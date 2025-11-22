using System.Security.Claims;

namespace FlowerShop.Services;

public abstract class BaseService<T> where T : class
{
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<T> _logger;

    protected BaseService(IHttpContextAccessor http, ILogger<T> logger)
    {
        _http = http;
        _logger = logger;
    }

    protected string? CurrentUserId => _http?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    protected string? CurrentUserName => _http?.HttpContext?.User?.Identity?.Name;
    
    private IDisposable? BeginUserScope()
    {

        return _logger.BeginScope(new
        {
            UserId = CurrentUserId ?? "*USER_ID_NULL*",
            UserName = CurrentUserName ?? "*USER_NAME_NULL*"
        });
    }
    
    protected void LogInfo(string message, params object[] args)
    {
        using (BeginUserScope())
            _logger.LogInformation(message, args);
        
    }

    protected void LogWarning(string message, params object[] args)
    {
        using (BeginUserScope())
            _logger.LogWarning(message, args);
    }

    protected void LogDebug(string message, params object[] args)
    {
        using (BeginUserScope())
            _logger.LogDebug(message, args);
    }

    protected void LogError(string message, Exception? ex = null, params object[] args)
    {
        using (BeginUserScope())
        {
            if(ex is not null)
                _logger.LogError(ex, message, args);
            else 
                _logger.LogError(message, args);
        }
    }
    
    protected void LogCritical(string message, Exception? ex = null, params object[] args)
    {
        using (BeginUserScope())
        {
            if(ex is not null)
                _logger.LogCritical(ex, message, args);
            else 
                _logger.LogCritical(message, args);
        }
    }
    
}