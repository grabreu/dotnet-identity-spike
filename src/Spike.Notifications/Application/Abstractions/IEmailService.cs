namespace Spike.Notifications.Application.Abstractions;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}

public record EmailMessage(string To, string Subject, string HtmlBody);
