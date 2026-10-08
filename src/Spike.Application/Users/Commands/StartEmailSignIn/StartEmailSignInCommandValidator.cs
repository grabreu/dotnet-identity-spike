namespace Spike.Application.Users.Commands.StartEmailSignIn;

public class StartEmailSignInCommandValidator : AbstractValidator<StartEmailSignInCommand>
{
    public StartEmailSignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();
    }
}
