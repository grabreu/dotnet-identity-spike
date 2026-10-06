namespace Spike.Users.Features;

public record SignInWithGoogleCommand(string IdToken) : ICommand<Result<TokenDto>>;

public class SignInWithGoogleHandler : ICommandHandler<SignInWithGoogleCommand, Result<TokenDto>>
{
    public ValueTask<Result<TokenDto>> Handle(SignInWithGoogleCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
            .WithName("SignInWithGoogle")
            .WithDisplayName("Sign In with Google")
            .WithSummary("Initiates the Google sign-in process for a user.")
            .WithDescription("This endpoint initiates the Google sign-in process for a user by validating the provided ID token.")
            .Produces<TokenDto>(StatusCodes.Status200OK);
    }

    private static ValueTask<IResult> HandleAsync(SignInWithGoogleRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new SignInWithGoogleCommand(request.IdToken), cancellationToken).ToOk();
    }
}
