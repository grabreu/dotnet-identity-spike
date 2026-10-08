using Spike.Users.Common;

namespace Spike.Users.Features;

public record SignInWithGoogleCommand(string IdToken) : ICommand<Result<TokenDto>>;

public class SignInWithGoogleHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<GoogleAuthOptions> options) : ICommandHandler<SignInWithGoogleCommand, Result<TokenDto>>
{
    private const string LoginProvider = "Google";

    public async ValueTask<Result<TokenDto>> Handle(SignInWithGoogleCommand command, CancellationToken cancellationToken)
    {
        var payload = await ValidateIdTokenAsync(command.IdToken);

        if (payload is null)
        {
            return Result.Unauthorized("Invalid Google ID token.");
        }

        var user = await userManager.FindByLoginAsync(LoginProvider, payload.Subject);

        if (user is not null)
        {
            return tokenService.GenerateToken(user);
        }

        user = await userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return createResult.ToErrors();
            }
        }

        var addLoginResult = await userManager.AddLoginAsync(user, new UserLoginInfo(LoginProvider, payload.Subject, LoginProvider));

        if (!addLoginResult.Succeeded)
        {
            return addLoginResult.ToErrors();
        }

        return tokenService.GenerateToken(user);
    }

    private async Task<GoogleJsonWebSignature.Payload?> ValidateIdTokenAsync(string idToken)
    {
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [options.Value.ClientId]
        };

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);

            return payload.EmailVerified ? payload : null;
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}

public record SignInWithGoogleRequest(string IdToken);

public static class SignInWithGoogleEndpoint
{
    public static RouteHandlerBuilder MapSignInWithGoogleEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/auth/google/sign-in", HandleAsync)
            .AllowAnonymous()
            .WithTags("Authentication")
            .WithName("SignInWithGoogle")
            .WithSummary("Sign In with Google")
            .WithDescription("Authenticates a user using a Google ID token.")
            .Produces<TokenDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static ValueTask<IResult> HandleAsync(SignInWithGoogleRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new SignInWithGoogleCommand(request.IdToken), cancellationToken).ToOk();
    }
}
