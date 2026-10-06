using Mediator;
using Spike.Notifications.Application.Abstractions;
using Spike.Users.Contracts.Events;

namespace Spike.Notifications.Application.EventHandlers;

public class UserRegisteredEventHandler(IEmailQueue emailQueue) : INotificationHandler<UserRegisteredEvent>
{
    public async ValueTask Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var message = new EmailMessage(
            notification.Email,
            "Welcome to Spike",
            "<h1>Welcome to Spike!</h1><p>Your account was created successfully.</p>");

        await emailQueue.EnqueueAsync(message, cancellationToken);
    }
}
