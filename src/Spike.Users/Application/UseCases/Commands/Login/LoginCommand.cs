using Desfecho;
using Mediator;
using Spike.Users.Application.Abstractions.Identity;

namespace Spike.Users.Application.UseCases.Commands.Login;

public record LoginCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;
