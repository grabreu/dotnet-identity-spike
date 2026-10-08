using Mediator;

namespace Spike.Application.FunctionalTests.Testing;

public abstract class FunctionalTest(ApplicationFixture fixture) : IAsyncLifetime
{
    protected FakeEmailQueue EmailQueue => fixture.EmailQueue;
    protected FakeGoogleTokenValidator GoogleTokenValidator => fixture.GoogleTokenValidator;
    protected UserSeeder Users => new(fixture.Services);

    public ValueTask InitializeAsync()
    {
        return fixture.ResetStateAsync();
    }

    protected async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command)
    {
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(command);
    }

    protected async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query)
    {
        using var scope = fixture.Services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(query);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
