namespace Spike.Application.Common.Email;

public interface IEmailQueue
{
    ValueTask EnqueueAsync(EmailMessage message, CancellationToken cancellationToken);
}

public record EmailMessage(string To, string Subject, string HtmlBody);
