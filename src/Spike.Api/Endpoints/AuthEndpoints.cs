using Spike.Application.Common.Identity;
using Spike.Application.Users.Commands.CompleteEmailSignIn;
using Spike.Application.Users.Commands.SignInWithGoogle;
using Spike.Application.Users.Commands.StartEmailSignIn;

namespace Spike.Api.Endpoints;

public record StartEmailSignInRequest(string Email);

public record CompleteEmailSignInRequest(string Email, string Code);

public record SignInWithGoogleRequest(string IdToken);

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/auth")
            .WithTags("Auth");

        group
            .MapPost("/email/sign-in", StartEmailSignInAsync)
            .AllowAnonymous()
            .WithName("StartEmailSignIn")
            .WithSummary("Start Email Sign-In")
            .WithDescription("Initiates the email sign-in process for a user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        group
            .MapPost("/email/sign-in/complete", CompleteEmailSignInAsync)
            .AllowAnonymous()
            .WithName("CompleteEmailSignIn")
            .WithSummary("Complete Email Sign-In")
            .WithDescription("Completes the email sign-in process using a verification code.")
            .Produces<TokenDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group
            .MapPost("/google/sign-in", SignInWithGoogleAsync)
            .AllowAnonymous()
            .WithName("SignInWithGoogle")
            .WithSummary("Sign In with Google")
            .WithDescription("Authenticates a user using a Google ID token.")
            .Produces<TokenDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static ValueTask<IResult> StartEmailSignInAsync(StartEmailSignInRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new StartEmailSignInCommand(request.Email), cancellationToken).ToNoContent();
    }

    private static ValueTask<IResult> CompleteEmailSignInAsync(CompleteEmailSignInRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new CompleteEmailSignInCommand(request.Email, request.Code), cancellationToken).ToOk();
    }

    private static ValueTask<IResult> SignInWithGoogleAsync(SignInWithGoogleRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new SignInWithGoogleCommand(request.IdToken), cancellationToken).ToOk();
    }
}
