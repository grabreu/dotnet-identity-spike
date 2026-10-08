namespace Spike.Users.Commands.StartEmailSignIn;

public record StartEmailSignInCommand(string Email) : ICommand<Result>;
