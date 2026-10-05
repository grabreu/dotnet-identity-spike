using Mediator;
using Microsoft.Extensions.Logging;
using Spike.Users.Contracts.Events;

namespace Spike.Notifications.Application.EventHandlers;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger) : INotificationHandler<UserRegisteredEvent>
{
    public ValueTask Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "User {UserId} ({Email}) registered at {Occurred:u}",
            notification.UserId,
            notification.Email,
            notification.Occurred);

        return ValueTask.CompletedTask;
    }
}
