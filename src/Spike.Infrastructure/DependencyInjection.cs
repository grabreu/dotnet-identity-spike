using Spike.Application.Common.Email;
using Spike.Application.Common.Identity;
using Spike.Infrastructure.Data;
using Spike.Infrastructure.Email;
using Spike.Infrastructure.Identity;

namespace Spike.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
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

    public static async Task EnsureDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
}
