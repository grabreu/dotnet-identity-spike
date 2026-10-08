using Spike.Application.Users.Queries.GetCurrentUser;

namespace Spike.Api.AcceptanceTests.Users;

[Collection(ApiCollection.Name)]
public class CurrentUserTests(ApiFactory factory) : AcceptanceTest(factory)
{
    [Fact]
    public async Task RequestWithoutTokenReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AuthenticatedRequestReturnsCurrentUser()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("user@spike.test");

        // Act
        var response = await client.GetAsync("/users/me");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var user = await response.Content.ReadFromJsonAsync<CurrentUserDto>();
        user!.Email.ShouldBe("user@spike.test");
        user.IsOnboarded.ShouldBeFalse();
    }
}
