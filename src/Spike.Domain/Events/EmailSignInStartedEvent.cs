namespace Spike.Domain.Events;

public record EmailSignInStartedEvent(string Email, string Code) : INotification;
