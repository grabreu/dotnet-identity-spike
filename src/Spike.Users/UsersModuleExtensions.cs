using System.Security.Claims;
using Desfecho.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spike.Users.Application.Abstractions.Identity;
using Spike.Users.Application.UseCases.Commands.Login;
using Spike.Users.Application.UseCases.Commands.Register;
using Spike.Users.Infrastructure.Identity;
using Spike.Users.Infrastructure.Persistence;

namespace Spike.Users;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("UsersDb")));

        services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<UsersDbContext>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        services.AddAuthorization();

        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    public static async Task InitializeUsersModuleAsync(this IHost app)
    {
        await UsersDbSeeder.SeedAsync(app.Services);
    }

    public static IEndpointRouteBuilder MapUsersModuleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/identity")
            .WithTags("Identity");

        group.MapPost("/register", async (RegisterCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToOk();
        })
        .WithName("Register");

        group.MapPost("/login", async (LoginCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);
            return result.ToOk();
        })
        .WithName("Login");

        group.MapGet("/me", (ClaimsPrincipal user) => Results.Ok(new
        {
            Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = user.FindFirstValue(ClaimTypes.Email)
        }))
        .RequireAuthorization()
        .WithName("GetCurrentUser");

        return app;
    }
}
