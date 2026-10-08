using Spike.Application.Common.Identity;
using Spike.Domain.Events;

namespace Spike.Application.Users.Commands.StartEmailSignIn;

public class StartEmailSignInCommandHandler(UserManager<ApplicationUser> userManager, IPublisher publisher) : ICommandHandler<StartEmailSignInCommand, Result>
{
    public const string Purpose = "PasswordlessLogin";

    public async ValueTask<Result> Handle(StartEmailSignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = command.Email,
                Email = command.Email
            };

            var createResult = await userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                return createResult.ToErrors();
            }
        }

        var code = await userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultEmailProvider, Purpose);

        await publisher.Publish(new EmailSignInStartedEvent(user.Email!, code), cancellationToken);

        return Result.Success();
    }
}
