namespace Spike.Application.Users.Commands.SignInWithGoogle;

public class SignInWithGoogleCommandValidator : AbstractValidator<SignInWithGoogleCommand>
{
    public SignInWithGoogleCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty();
    }
}
