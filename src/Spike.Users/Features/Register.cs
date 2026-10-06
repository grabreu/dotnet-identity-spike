using Desfecho;
using Desfecho.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Spike.Users.Common;
using Spike.Users.Contracts.Events;

namespace Spike.Users.Features;

public record RegisterCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;

public class RegisterCommandHandler(UserManager<IdentityUser> userManager, ITokenService tokenService, IPublisher publisher) : ICommandHandler<RegisterCommand, Result<TokenResponse>>
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

        await publisher.Publish(new UserRegisteredEvent(user.Id, user.Email, DateTimeOffset.UtcNow), cancellationToken);

        return await tokenService.GenerateTokenAsync(user, cancellationToken);
    }
}

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToOk();
        })
        .WithName("Register")
        .WithTags("Auth")
        .WithSummary("Register user")
        .WithDescription("Registers a new user and returns an access token.")
        .Produces<TokenResponse>()
        .ProducesValidationProblem();

        return app;
    }
}
