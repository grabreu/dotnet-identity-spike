using Spike.Application.Common.Identity;

namespace Spike.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.DisplayName)
            .HasMaxLength(100);
    }
}
