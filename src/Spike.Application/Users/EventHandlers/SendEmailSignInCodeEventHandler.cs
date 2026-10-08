using Spike.Application.Common.Email;
using Spike.Domain.Events;

namespace Spike.Application.Users.EventHandlers;

public class SendEmailSignInCodeEventHandler(IEmailQueue emailQueue) : INotificationHandler<EmailSignInStartedEvent>
{
    public async ValueTask Handle(EmailSignInStartedEvent notification, CancellationToken cancellationToken)
    {
        var message = new EmailMessage(
            notification.Email,
            "Your Spike sign-in code",
            $"<p>Your sign-in code is <strong>{notification.Code}</strong>.</p><p>It expires in a few minutes. If you did not request it, ignore this email.</p>");

        await emailQueue.EnqueueAsync(message, cancellationToken);
    }
}
