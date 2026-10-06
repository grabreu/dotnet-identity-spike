using Microsoft.Extensions.Hosting;

namespace Spike.Notifications;

public static class NotificationsModuleExtensions
{
    public static IHostApplicationBuilder AddNotificationsModuleServices(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}
