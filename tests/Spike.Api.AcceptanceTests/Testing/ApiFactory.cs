using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Spike.Notifications.Email;
using Spike.Users.Identity;
using Testcontainers.MsSql;

namespace Spike.Api.AcceptanceTests.Testing;

public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private Respawner? _respawner;

    public FakeEmailQueue EmailQueue { get; } = new();

    public FakeGoogleTokenValidator GoogleTokenValidator { get; } = new();

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        // Building the host (first access to Services) applies the migrations: Program does it in Development.
        _ = Services;

        await using var connection = await OpenConnectionAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async ValueTask ResetStateAsync()
    {
        await using var connection = await OpenConnectionAsync();

        await _respawner!.ResetAsync(connection);

        EmailQueue.Clear();
        GoogleTokenValidator.Clear();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:SpikeDb", _database.GetConnectionString());
        builder.UseSetting("Jwt:Issuer", "Spike.Tests");
        builder.UseSetting("Jwt:Audience", "Spike.Tests");
        builder.UseSetting("Jwt:SecretKey", new string('k', 64));
        builder.UseSetting("Authentication:Google:ClientId", "test-client-id");
        builder.UseSetting("Email:Host", "localhost");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IEmailQueue>();
            services.AddSingleton<IEmailQueue>(EmailQueue);

            services.RemoveAll<IGoogleTokenValidator>();
            services.AddSingleton<IGoogleTokenValidator>(GoogleTokenValidator);
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        await _database.DisposeAsync();
    }

    private async Task<SqlConnection> OpenConnectionAsync()
    {
        var connection = new SqlConnection(_database.GetConnectionString());

        await connection.OpenAsync();

        return connection;
    }
}
