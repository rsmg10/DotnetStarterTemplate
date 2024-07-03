using Infra.Authentication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence.Db;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, RoleType, Guid>(options)  // add user role and key  
{
    public override DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(
                "Data Source=.;Initial Catalog=StarterTemplate;Integrated Security=True;TrustServerCertificate=True");
    }
}