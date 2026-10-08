using System.Globalization;
using System.Reflection;
using Spike.Users.Data;
using Spike.Users.Endpoints;
using Spike.Users.Identity;

namespace Spike.Users;

public static class UsersModule
{
    public static IHostApplicationBuilder AddUsersModule(this IHostApplicationBuilder builder)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");

        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.AddSqlServerDbContext<ApplicationDbContext>("SpikeDb");

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddOptions<GoogleAuthOptions>()
            .BindConfiguration(GoogleAuthOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        builder.Services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, options) =>
            {
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = options.Value.Issuer,
                    ValidAudience = options.Value.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

        return builder;
    }

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
        endpoints.MapProfileEndpoints();

        return endpoints;
    }

    public static async Task EnsureUsersDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
}
