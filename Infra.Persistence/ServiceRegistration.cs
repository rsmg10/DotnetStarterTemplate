using Infra.Authentication;
using Infra.Persistence.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Persistence;

public static class ServiceRegistration
{
    public static void AddDb(this IServiceCollection services, IConfiguration configuration)
    { 
        // services.AddDbContext<AppDbContext>(options => options.UseSqlServer("Data Source=.;Initial Catalog=StarterTemplate;Integrated Security=True;TrustServerCertificate=True"));
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(configuration.GetConnectionString("Database")), contextLifetime: ServiceLifetime.Transient);

        services.AddHostedService<DatabaseMigratorHostedService>();
        services.AddUserAuth(configuration)
            .AddEntityFrameworkStores<AppDbContext>();
        // .AddTokenProvider("email", typeof(CustomEmailTokenProvider))
        // .AddDefaultTokenProviders();

    }
}