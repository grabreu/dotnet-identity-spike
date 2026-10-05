using Desfecho;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Spike.Users.Application.Abstractions.Identity;

namespace Spike.Users.Application.UseCases.Commands.Login;

public class LoginCommandHandler(UserManager<IdentityUser> userManager, ITokenService tokenService) : ICommandHandler<LoginCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, command.Password))
        {
            return Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
        }

        return await tokenService.GenerateTokenAsync(user, cancellationToken);
    }
}
