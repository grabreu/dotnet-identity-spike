using Microsoft.AspNetCore.Identity;

namespace Spike.Users.Application.Abstractions.Identity;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(IdentityUser user, CancellationToken cancellationToken);
}

public record TokenResponse(string AccessToken);
