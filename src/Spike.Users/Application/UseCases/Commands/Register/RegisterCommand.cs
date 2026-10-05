using Desfecho;
using Mediator;
using Spike.Users.Application.Abstractions.Identity;

namespace Spike.Users.Application.UseCases.Commands.Register;

public record RegisterCommand(string Email, string Password) : ICommand<Result<TokenResponse>>;
