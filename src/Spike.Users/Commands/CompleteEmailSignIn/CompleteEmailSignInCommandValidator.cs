namespace Spike.Users.Commands.CompleteEmailSignIn;

public class CompleteEmailSignInCommandValidator : AbstractValidator<CompleteEmailSignInCommand>
{
    public CompleteEmailSignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(@"^\d{6}$");
    }
}
