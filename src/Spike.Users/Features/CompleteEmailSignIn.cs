namespace Spike.Users.Features;

public record TokenDto(string AccessToken);

public record CompleteEmailSignInCommand(string Email, string Code) : ICommand<Result<TokenDto>>;

public class CompleteEmailSignInHandler : ICommandHandler<CompleteEmailSignInCommand, Result<TokenDto>>
{
    public ValueTask<Result<TokenDto>> Handle(CompleteEmailSignInCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public record CompleteEmailSignInRequest(string Email, string Code);

public static class CompleteEmailSignInEndpoint
{
    public static RouteHandlerBuilder MapCompleteEmailSignInEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/auth/email/sign-in/complete", HandleAsync)
            .AllowAnonymous()
            .WithName("CompleteEmailSignIn")
            .WithDisplayName("Complete Email Sign-In")
            .WithSummary("Completes the email sign-in process for a user.")
            .WithDescription("This endpoint completes the email sign-in process for a user by verifying the provided code.")
            .Produces<TokenDto>(StatusCodes.Status200OK);
    }

    private static ValueTask<IResult> HandleAsync(CompleteEmailSignInRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new CompleteEmailSignInCommand(request.Email, request.Code), cancellationToken).ToOk();
    }
}
