using Desfecho;
using Desfecho.AspNetCore;
using Google.Apis.Auth;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Spike.Users.Common;
using Spike.Users.Contracts.Events;

namespace Spike.Users.Features;

public record GoogleLoginCommand(string IdToken) : ICommand<Result<TokenResponse>>;

public class GoogleLoginCommandHandler(
    UserManager<IdentityUser> userManager,
    ITokenService tokenService,
    IPublisher publisher,
    IOptions<GoogleOptions> options) : ICommandHandler<GoogleLoginCommand, Result<TokenResponse>>
{
    private const string Provider = "Google";

    public async ValueTask<Result<TokenResponse>> Handle(GoogleLoginCommand command, CancellationToken cancellationToken)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(command.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [options.Value.ClientId]
            });
        }
        catch (InvalidJwtException)
        {
            return Error.Unauthorized("Auth.InvalidGoogleToken", "Invalid Google token.");
        }

        if (!payload.EmailVerified)
        {
            return Error.Unauthorized("Auth.GoogleEmailNotVerified", "Google email is not verified.");
        }

        var user = await userManager.FindByLoginAsync(Provider, payload.Subject);

        if (user is not null)
        {
            return await tokenService.GenerateTokenAsync(user, cancellationToken);
        }

        if (await userManager.FindByEmailAsync(payload.Email) is not null)
        {
            return Error.Conflict("Auth.EmailAlreadyRegistered", "This email is already registered with a password.");
        }

        user = new IdentityUser
        {
            Email = payload.Email,
            UserName = payload.Email,
            EmailConfirmed = true
        };

        var created = await userManager.CreateAsync(user);

        if (!created.Succeeded)
        {
            return created.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        var linked = await userManager.AddLoginAsync(user, new UserLoginInfo(Provider, payload.Subject, Provider));

        if (!linked.Succeeded)
        {
            return linked.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
        }

        await publisher.Publish(new UserRegisteredEvent(user.Id, payload.Email, DateTimeOffset.UtcNow), cancellationToken);

        return await tokenService.GenerateTokenAsync(user, cancellationToken);
    }
}

public static class GoogleLoginEndpoint
{
    public static IEndpointRouteBuilder MapGoogleLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/google", async (GoogleLoginCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToOk();
        })
        .WithName("GoogleLogin")
        .WithTags("Auth")
        .WithSummary("Log in with Google")
        .WithDescription("Authenticates a user with a Google ID token and returns an access token.")
        .Produces<TokenResponse>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
