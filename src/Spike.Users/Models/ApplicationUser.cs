namespace Spike.Users.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public DateTimeOffset? OnboardedAt { get; set; }
}
