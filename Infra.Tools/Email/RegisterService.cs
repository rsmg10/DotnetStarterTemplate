using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Tools.Email;

public static class RegisterService
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailSender>();
        services.Configure<SmtpOption>(configuration.GetSection(SmtpOption.SectionName));
        return services;
    }
    
}