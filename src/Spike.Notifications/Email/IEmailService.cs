using Spike.Notifications.Email;

namespace Spike.Notifications.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}
