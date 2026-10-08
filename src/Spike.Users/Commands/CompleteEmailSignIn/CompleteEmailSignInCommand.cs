using Spike.Users.Identity;

namespace Spike.Users.Commands.CompleteEmailSignIn;

public record CompleteEmailSignInCommand(string Email, string Code) : ICommand<Result<TokenDto>>;
