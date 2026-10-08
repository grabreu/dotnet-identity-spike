using Mediator;

namespace Spike.Contracts;

public record EmailSignInStartedEvent(string Email, string Code) : INotification;
