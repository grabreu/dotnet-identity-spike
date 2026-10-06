using System.ComponentModel.DataAnnotations;

namespace Spike.Users.Common;

public class GoogleOptions
{
    public const string SectionName = "Google";

    [Required] public string ClientId { get; init; } = "";
}
