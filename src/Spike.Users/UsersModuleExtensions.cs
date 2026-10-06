using System.Security.Claims;
using Desfecho.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
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
    public static IHostApplicationBuilder AddUsersModuleServices(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<UsersDbContext>("UsersDb");

        builder.Services.AddIdentityCore<IdentityUser>(options =>
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

        builder.Services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        builder.Services.AddAuthorization();
        builder.Services.ConfigureOptions<ConfigureJwtBearerOptions>();

        builder.Services.AddScoped<ITokenService, TokenService>();

        return builder;
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
