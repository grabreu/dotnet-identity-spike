using Spike.Users.Identity;

namespace Spike.Users.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityUserContext<ApplicationUser, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
