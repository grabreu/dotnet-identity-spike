using Mediator;

namespace Spike.Users.Contracts.Events;

public record UserRegisteredEvent(string UserId, string Email, DateTimeOffset Occurred) : INotification;
