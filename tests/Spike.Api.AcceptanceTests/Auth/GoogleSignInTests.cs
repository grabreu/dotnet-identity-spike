using Spike.Application.Common.Identity;

namespace Spike.Api.AcceptanceTests.Auth;

[Collection(ApiCollection.Name)]
public class GoogleSignInTests(ApiFactory factory) : AcceptanceTest(factory)
{
    [Fact]
    public async Task ValidGoogleTokenReturnsToken()
    {
        // Arrange
        GoogleTokenValidator.Accept("token", new GoogleUserInfo(Guid.NewGuid().ToString(), "user@spike.test"));

        // Act
        var response = await Client.PostAsJsonAsync("/auth/google/sign-in", new { IdToken = "token" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        token!.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvalidGoogleTokenReturnsUnauthorized()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/auth/google/sign-in", new { IdToken = "unknown" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task EmptyGoogleTokenReturnsBadRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/auth/google/sign-in", new { IdToken = "" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
