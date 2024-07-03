using Infra.Persistence.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infra.Persistence;

public class DatabaseMigratorHostedService(IServiceProvider serviceProvider) : IHostedService
{
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
            using IServiceScope scope = serviceProvider.CreateScope();
            AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync(cancellationToken);
            
        }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}