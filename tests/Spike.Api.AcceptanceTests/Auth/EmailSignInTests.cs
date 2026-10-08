using Spike.Users.Identity;

namespace Spike.Api.AcceptanceTests.Auth;

[Collection(ApiCollection.Name)]
public class EmailSignInTests(ApiFactory factory) : AcceptanceTest(factory)
{
    [Fact]
    public async Task StartingSignInReturnsNoContentAndSendsCode()
    {
        // Arrange
        var email = "user@spike.test";

        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = email });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        EmailQueue.Messages.ShouldHaveSingleItem().To.ShouldBe(email);
    }

    [Fact]
    public async Task StartingSignInWithInvalidEmailReturnsBadRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = "not-an-email" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompletingSignInWithValidCodeReturnsToken()
    {
        // Arrange
        var email = "user@spike.test";
        await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = email });

        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = email, Code = EmailQueue.LastCodeSentTo(email) });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var token = await response.Content.ReadFromJsonAsync<TokenDto>();
        token!.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CompletingSignInWithWrongCodeReturnsUnauthorized()
    {
        // Arrange
        var email = "user@spike.test";
        await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = email });
        var wrongCode = EmailQueue.LastCodeSentTo(email) == "000000" ? "111111" : "000000";

        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = email, Code = wrongCode });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompletingSignInWithMalformedCodeReturnsBadRequest()
    {
        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = "user@spike.test", Code = "123" });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ReusingACodeReturnsUnauthorized()
    {
        // Arrange
        var email = "user@spike.test";
        await Client.PostAsJsonAsync("/auth/email/sign-in", new { Email = email });
        var code = EmailQueue.LastCodeSentTo(email);
        await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = email, Code = code });

        // Act
        var response = await Client.PostAsJsonAsync("/auth/email/sign-in/complete", new { Email = email, Code = code });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
