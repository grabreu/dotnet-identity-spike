using Spike.Application.Common.Identity;

namespace Spike.Application.Users.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(UserManager<ApplicationUser> userManager) : ICommandHandler<CompleteOnboardingCommand, Result>
{
    public async ValueTask<Result> Handle(CompleteOnboardingCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        if (user.OnboardedAt is not null)
        {
            return Result.Conflict("User has already completed onboarding.");
        }

        user.DisplayName = command.DisplayName.Trim();
        user.OnboardedAt = DateTimeOffset.UtcNow;

        var updateResult = await userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return updateResult.ToErrors();
        }

        return Result.Success();
    }
}
