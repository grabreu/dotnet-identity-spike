using System.ComponentModel.DataAnnotations;

namespace Spike.Notifications.Infrastructure.Email;

public class EmailOptions
{
    public const string SectionName = "Email";

    [Required] public string Host { get; init; } = "";
    [Range(1, 65535)] public int Port { get; init; }
    [Required, EmailAddress] public string FromAddress { get; init; } = "";
    [Required] public string FromName { get; init; } = "";
    public string Username { get; init; } = "";
    public string Password { get; init; } = "";
}
