using System.ComponentModel.DataAnnotations;

namespace Spike.Users.Common;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required] public string Issuer { get; init; } = "";
    [Required] public string Audience { get; init; } = "";
    [Required, MinLength(32)] public string SecretKey { get; init; } = "";
    [Range(1, 1440)] public int AccessTokenExpirationMinutes { get; init; }
}
