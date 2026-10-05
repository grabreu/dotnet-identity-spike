using Desfecho;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Spike.Users.Application.Abstractions.Identity;

namespace Spike.Users.Application.UseCases.Commands.Register;

public class RegisterCommandHandler(UserManager<IdentityUser> userManager, ITokenService tokenService) : ICommandHandler<RegisterCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var user = new IdentityUser
        {
            Email = command.Email,
            UserName = command.Email
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        return await tokenService.GenerateTokenAsync(user, cancellationToken);
    }
}
