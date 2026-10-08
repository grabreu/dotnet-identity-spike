using Spike.Application.Common.Email;

namespace Spike.Infrastructure.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken);
}
