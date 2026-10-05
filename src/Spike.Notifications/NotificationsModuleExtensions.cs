using Microsoft.Extensions.DependencyInjection;

namespace Spike.Notifications;

public static class NotificationsModuleExtensions
{
    public static IServiceCollection AddNotificationsModuleServices(this IServiceCollection services)
    {
        return services;
    }
}
