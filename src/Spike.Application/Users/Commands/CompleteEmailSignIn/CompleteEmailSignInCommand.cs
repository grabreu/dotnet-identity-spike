using Spike.Application.Common.Identity;

namespace Spike.Application.Users.Commands.CompleteEmailSignIn;

public record CompleteEmailSignInCommand(string Email, string Code) : ICommand<Result<TokenDto>>;
