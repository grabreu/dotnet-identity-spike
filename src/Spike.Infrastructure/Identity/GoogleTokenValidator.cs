using Spike.Application.Common.Identity;

namespace Spike.Infrastructure.Identity;

public class GoogleTokenValidator(IOptions<GoogleAuthOptions> options) : IGoogleTokenValidator
{
    public async Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken cancellationToken)
    {
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [options.Value.ClientId]
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

            return payload.EmailVerified ? new GoogleUserInfo(payload.Subject, payload.Email) : null;
        }
        catch (Exception exception) when (exception is InvalidJwtException or Newtonsoft.Json.JsonException)
        {
            return null;
        }
    }
}
