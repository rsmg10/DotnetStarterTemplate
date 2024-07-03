using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Tools.Cache;

public static class RegisterService
{
    public static void AddCache(this IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
    }
    
}