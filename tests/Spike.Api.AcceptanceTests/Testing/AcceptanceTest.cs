using System.Net.Http.Headers;
using Spike.Users.Identity;

namespace Spike.Api.AcceptanceTests.Testing;

public abstract class AcceptanceTest(ApiFactory factory) : IAsyncLifetime
{
    protected HttpClient Client { get; } = factory.CreateClient();

    protected FakeEmailQueue EmailQueue => factory.EmailQueue;

    protected FakeGoogleTokenValidator GoogleTokenValidator => factory.GoogleTokenValidator;

    public ValueTask InitializeAsync()
    {
        return factory.ResetStateAsync();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync(string email)
    {
        await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = email });

        var response = await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = email, Code = EmailQueue.LastCodeSentTo(email) });
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token!.AccessToken);

        return client;
    }
}
