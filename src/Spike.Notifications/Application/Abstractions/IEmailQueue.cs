namespace Spike.Notifications.Application.Abstractions;

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken);
}
