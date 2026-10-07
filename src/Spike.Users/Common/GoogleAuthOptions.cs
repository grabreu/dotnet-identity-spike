namespace Spike.Users.Common;

public class GoogleAuthOptions
{
    public const string SectionName = "Authentication:Google";

    [Required] public string ClientId { get; init; } = string.Empty;
}
