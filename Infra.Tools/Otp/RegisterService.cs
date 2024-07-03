using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Tools.Otp;

public static class RegisterService
{
    public static void AddOtp(this IServiceCollection services)
    {
        services.AddScoped<IOtpService, OtpService>();
    }
    
}