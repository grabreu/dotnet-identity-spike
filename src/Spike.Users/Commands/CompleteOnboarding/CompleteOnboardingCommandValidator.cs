namespace Spike.Users.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandValidator : AbstractValidator<CompleteOnboardingCommand>
{
    public CompleteOnboardingCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
