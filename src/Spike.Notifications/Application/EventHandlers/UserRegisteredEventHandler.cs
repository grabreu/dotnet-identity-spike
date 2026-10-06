using Mediator;
using Microsoft.Extensions.Logging;
using Spike.Notifications.Application.Abstractions;
using Spike.Users.Contracts.Events;

namespace Spike.Notifications.Application.EventHandlers;

public class UserRegisteredEventHandler(IEmailQueue emailQueue, ILogger<UserRegisteredEventHandler> logger) : INotificationHandler<UserRegisteredEvent>
{
    public async ValueTask Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "User {UserId} ({Email}) registered at {Occurred:u}",
            notification.UserId,
            notification.Email,
            notification.Occurred);

        var message = new EmailMessage(
            notification.Email,
            "Welcome to Spike",
            "<h1>Welcome to Spike!</h1><p>Your account was created successfully.</p>");

        await emailQueue.EnqueueAsync(message, cancellationToken);
    }
}
