using Spike.Users.Identity;
using Spike.Users.Commands.StartEmailSignIn;

namespace Spike.Users.Commands.CompleteEmailSignIn;

public class CompleteEmailSignInCommandHandler(UserManager<ApplicationUser> userManager, ITokenService tokenService) : ICommandHandler<CompleteEmailSignInCommand, Result<TokenDto>>
{
    public async ValueTask<Result<TokenDto>> Handle(CompleteEmailSignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null || await userManager.IsLockedOutAsync(user))
        {
            return Result.Unauthorized("Invalid or expired code.");
        }

        if (!await userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultEmailProvider, StartEmailSignInCommandHandler.Purpose, command.Code))
        {
            await userManager.AccessFailedAsync(user);

            return Result.Unauthorized("Invalid or expired code.");
        }

        await userManager.ResetAccessFailedCountAsync(user);

        user.EmailConfirmed = true;

        // Codes are stateless and valid for several minutes: rotating the stamp makes each one single-use.
        var stampResult = await userManager.UpdateSecurityStampAsync(user);

        if (!stampResult.Succeeded)
        {
            return Result.Unauthorized("Invalid or expired code.");
        }

        return tokenService.GenerateToken(user);
    }
}
