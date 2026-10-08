using Spike.Users.Queries.GetCurrentUser;

namespace Spike.Api.AcceptanceTests.Users;

[Collection(ApiCollection.Name)]
public class OnboardingTests(ApiFactory factory) : AcceptanceTest(factory)
{
    [Fact]
    public async Task RequestWithoutTokenReturnsUnauthorized()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/users/me/onboarding", new { DisplayName = "Ada" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompletingOnboardingReturnsNoContentAndMarksUserAsOnboarded()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("user@spike.test");

        // Act
        var response = await client.PostAsJsonAsync("/users/me/onboarding", new { DisplayName = "  Ada Lovelace  " });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var user = await client.GetFromJsonAsync<CurrentUserDto>("/users/me");
        user!.DisplayName.ShouldBe("Ada Lovelace");
        user.IsOnboarded.ShouldBeTrue();
    }

    [Fact]
    public async Task CompletingOnboardingTwiceReturnsConflict()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("user@spike.test");
        await client.PostAsJsonAsync("/users/me/onboarding", new { DisplayName = "Ada" });

        // Act
        var response = await client.PostAsJsonAsync("/users/me/onboarding", new { DisplayName = "Grace" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task BlankDisplayNameReturnsBadRequest()
    {
        // Arrange
        var client = await CreateAuthenticatedClientAsync("user@spike.test");

        // Act
        var response = await client.PostAsJsonAsync("/users/me/onboarding", new { DisplayName = "   " });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
