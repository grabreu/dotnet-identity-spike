using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Spike.Users.Application.Abstractions.Identity;

namespace Spike.Users.Infrastructure.Identity;

public class TokenService(UserManager<IdentityUser> userManager, IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _jwt = options.Value;

    public async Task<TokenResponse> GenerateTokenAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        var subject = new ClaimsIdentity(
        [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!)
        ]);

        var roles = await userManager.GetRolesAsync(user);
        subject.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = subject,
            Issuer = _jwt.Issuer,
            Audience = _jwt.Audience,
            Expires = expiresAt,
            SigningCredentials = credentials,
        };

        var accessToken = new JsonWebTokenHandler().CreateToken(descriptor);

        return new TokenResponse(accessToken);
    }
}
