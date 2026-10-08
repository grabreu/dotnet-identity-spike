using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Respawn;
using Spike.Application.Common.Email;
using Spike.Application.Common.Identity;
using Spike.Infrastructure;
using Testcontainers.MsSql;

namespace Spike.Application.FunctionalTests.Testing;

public class ApplicationFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _database = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private IHost? _host;
    private Respawner? _respawner;

    public FakeEmailQueue EmailQueue { get; } = new();
    public FakeGoogleTokenValidator GoogleTokenValidator { get; } = new();
    public IServiceProvider Services => _host!.Services;

    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        var builder = Host.CreateApplicationBuilder();

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SpikeDb"] = _database.GetConnectionString(),
            ["Jwt:Issuer"] = "Spike.Tests",
            ["Jwt:Audience"] = "Spike.Tests",
            ["Jwt:SecretKey"] = new string('k', 64),
            ["Jwt:AccessTokenExpirationMinutes"] = "15",
            ["Authentication:Google:ClientId"] = "test-client-id",
            ["Email:Host"] = "localhost",
            ["Email:Port"] = "25",
            ["Email:FromAddress"] = "no-reply@spike.test",
            ["Email:FromName"] = "Spike"
        });

        builder.AddApplicationServices();
        builder.AddInfrastructureServices();

        builder.Services.RemoveAll<IEmailQueue>();
        builder.Services.AddSingleton<IEmailQueue>(EmailQueue);

        builder.Services.RemoveAll<IGoogleTokenValidator>();
        builder.Services.AddSingleton<IGoogleTokenValidator>(GoogleTokenValidator);

        _host = builder.Build();

        await _host.EnsureDatabaseAsync();

        await using var connection = new SqlConnection(_database.GetConnectionString());
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }

    public async ValueTask ResetStateAsync()
    {
        await using var connection = new SqlConnection(_database.GetConnectionString());
        await connection.OpenAsync();

        await _respawner!.ResetAsync(connection);

        EmailQueue.Clear();
        GoogleTokenValidator.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        _host?.Dispose();

        await _database.DisposeAsync();

        GC.SuppressFinalize(this);
    }
}
