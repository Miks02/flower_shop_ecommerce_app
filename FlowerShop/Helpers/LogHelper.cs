using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FlowerShop.Helpers;

public static class LogHelper
{
    public static void LogModelErrors(ModelStateDictionary modelState, ILogger logger, string? userMessage)
    {

        if(userMessage != null) logger.LogWarning(userMessage);
           
        foreach(var key in modelState.Keys)
        {
            var state = modelState[key];
            if (state == null) continue;

            if(state.Errors.Any())
            {
                logger.LogInformation($"Greška u polju: {key}");
                foreach(var error in state.Errors)
                {
                    logger.LogInformation($" - {error}");
                }
            }
        }

    }
}