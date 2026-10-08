using Spike.Users.Identity;

namespace Spike.Users.FunctionalTests.Testing.Fakes;

public class FakeGoogleTokenValidator : IGoogleTokenValidator
{
    private readonly Dictionary<string, GoogleUserInfo> _users = [];

    public void Accept(string idToken, GoogleUserInfo user)
    {
        _users[idToken] = user;
    }

    public Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken cancellationToken)
    {
        return Task.FromResult(_users.GetValueOrDefault(idToken));
    }

    public void Clear()
    {
        _users.Clear();
    }
}
