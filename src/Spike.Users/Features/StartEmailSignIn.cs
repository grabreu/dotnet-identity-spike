namespace Spike.Users.Features;

public record StartEmailSignInCommand(string Email) : ICommand<Result>;

public class StartEmailSignInHandler : ICommandHandler<StartEmailSignInCommand, Result>
{
    public ValueTask<Result> Handle(StartEmailSignInCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public record StartEmailSignInRequest(string Email);

public static class StartEmailSignInEndpoint
{
    public static RouteHandlerBuilder MapStartEmailSignInEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/auth/email/sign-in", HandleAsync)
            .AllowAnonymous()
            .WithName("StartEmailSignIn")
            .WithDisplayName("Start Email Sign-In")
            .WithSummary("Initiates the email sign-in process for a user.")
            .WithDescription("This endpoint initiates the email sign-in process for a user by sending a verification email.")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static ValueTask<IResult> HandleAsync(StartEmailSignInRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new StartEmailSignInCommand(request.Email), cancellationToken).ToNoContent();
    }
}
