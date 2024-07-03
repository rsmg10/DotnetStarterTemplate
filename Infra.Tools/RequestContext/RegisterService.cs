using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Tools.RequestContext;

public static class RegisterService
{
    public static void AddRequestContext(this IServiceCollection services)
    {
        services.AddScoped<IRequestContext, RequestContext>();
    }
    
}