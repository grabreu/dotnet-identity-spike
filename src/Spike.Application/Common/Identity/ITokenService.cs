namespace Spike.Application.Common.Identity;

public interface ITokenService
{
    TokenDto GenerateToken(ApplicationUser user);
}

public record TokenDto(string AccessToken);
