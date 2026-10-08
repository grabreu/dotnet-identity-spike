namespace Spike.Users.Identity;

public interface IGoogleTokenValidator
{
    Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken cancellationToken);
}

public record GoogleUserInfo(string Subject, string Email);
