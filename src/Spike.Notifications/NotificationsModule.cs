using Spike.Notifications.Email;

namespace Spike.Notifications;

public static class NotificationsModule
{
    public static IHostApplicationBuilder AddNotificationsModule(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<EmailOptions>()
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
