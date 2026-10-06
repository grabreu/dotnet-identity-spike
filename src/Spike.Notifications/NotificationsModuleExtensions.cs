using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spike.Notifications.Email;

namespace Spike.Notifications;

public static class NotificationsModuleExtensions
{
    public static IHostApplicationBuilder AddNotificationsModuleServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
        builder.Services.AddSingleton<ChannelEmailQueue>();
        builder.Services.AddSingleton<IEmailQueue>(services => services.GetRequiredService<ChannelEmailQueue>());
        builder.Services.AddHostedService<EmailSendingWorker>();

        return builder;
    }
}
