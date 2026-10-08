namespace Spike.Application.Users.Commands.StartEmailSignIn;

public record StartEmailSignInCommand(string Email) : ICommand<Result>;
