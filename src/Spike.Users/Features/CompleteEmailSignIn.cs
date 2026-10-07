using Spike.Users.Common;

namespace Spike.Users.Features;

public record CompleteEmailSignInCommand(string Email, string Code) : ICommand<Result<TokenDto>>;

public class CompleteEmailSignInHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService) : ICommandHandler<CompleteEmailSignInCommand, Result<TokenDto>>
{
    private const string Purpose = "PasswordlessLogin";

    public async ValueTask<Result<TokenDto>> Handle(CompleteEmailSignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null || !await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, Purpose, command.Code))
        {
            return Result.Unauthorized("Invalid or expired code.");
        }

        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToErrors();
            }
        }

        return tokenService.GenerateToken(user);
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
            .WithTags("Authentication")
            .WithName("CompleteEmailSignIn")
            .WithSummary("Complete Email Sign-In")
            .WithDescription("Completes the email sign-in process using a verification code.")
            .Produces<TokenDto>(StatusCodes.Status200OK);
    }

    private static ValueTask<IResult> HandleAsync(CompleteEmailSignInRequest request, ISender sender, CancellationToken cancellationToken)
    {
        return sender.Send(new CompleteEmailSignInCommand(request.Email, request.Code), cancellationToken).ToOk();
    }
}
