using System.Threading.RateLimiting;
using FlowerShop.Infrastructure.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FlowerShop.Infrastructure.Extensions;

public static class RateLimitingRegistration
{
    public static void AddGlobalRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                
                return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 100,
                    TokensPerPeriod = 10,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(15),
                    AutoReplenishment = true
                });
            });

            limiterOptions.OnRejected = (context, _) =>
            {
                context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter);
                return RateLimitingRejectionHandler.HandleAsync(context.HttpContext, retryAfter);
            };
        });
    }
}
