using Spike.Application.Common.Identity;

namespace Spike.Application.Users.Commands.SignInWithGoogle;

public class SignInWithGoogleCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService, IGoogleTokenValidator googleTokenValidator) : ICommandHandler<SignInWithGoogleCommand, Result<TokenDto>>
{
    private const string LoginProvider = "Google";

    public async ValueTask<Result<TokenDto>> Handle(SignInWithGoogleCommand command, CancellationToken cancellationToken)
    {
        var googleUser = await googleTokenValidator.ValidateAsync(command.IdToken, cancellationToken);

        if (googleUser is null)
        {
            return Result.Unauthorized("Invalid Google ID token.");
        }

        var user = await userManager.FindByLoginAsync(LoginProvider, googleUser.Subject);

        if (user is not null)
        {
            return tokenService.GenerateToken(user);
        }

        user = await userManager.FindByEmailAsync(googleUser.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = googleUser.Email,
                Email = googleUser.Email
            };

            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return createResult.ToErrors();
            }
        }

        // Google only hands out verified emails, so linking also confirms the email of an existing account.
        user.EmailConfirmed = true;

        var addLoginResult = await userManager.AddLoginAsync(user, new UserLoginInfo(LoginProvider, googleUser.Subject, LoginProvider));

        if (!addLoginResult.Succeeded)
        {
            return addLoginResult.ToErrors();
        }

        return tokenService.GenerateToken(user);
    }
}
