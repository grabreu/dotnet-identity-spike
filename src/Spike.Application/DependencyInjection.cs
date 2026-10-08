using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spike.Application.Common.Behaviors;

namespace Spike.Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors =
            [
                typeof(ValidationBehavior<,>)
            ];
        });

        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return builder;
    }
}
