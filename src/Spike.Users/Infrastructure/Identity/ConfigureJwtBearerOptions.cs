using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Spike.Users.Infrastructure.Identity;

public class ConfigureJwtBearerOptions(IOptions<JwtOptions> options) : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwt = options.Value;

    public void Configure(string? name, JwtBearerOptions bearer)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        Configure(bearer);
    }

    public void Configure(JwtBearerOptions bearer)
    {
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwt.Issuer,
            ValidAudience = _jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    }
}
