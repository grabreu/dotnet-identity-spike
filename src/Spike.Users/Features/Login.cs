using Desfecho;
using Desfecho.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Spike.Users.Common;

namespace Spike.Users.Features;

public record LoginCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;

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

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToOk();
        })
        .WithName("Login")
        .WithTags("Auth")
        .WithSummary("Log in")
        .WithDescription("Authenticates a user and returns an access token.")
        .Produces<TokenResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
