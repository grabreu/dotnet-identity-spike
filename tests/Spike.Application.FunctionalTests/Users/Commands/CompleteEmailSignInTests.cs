using Spike.Application.Users.Commands.CompleteEmailSignIn;
using Spike.Application.Users.Commands.StartEmailSignIn;

namespace Spike.Application.FunctionalTests.Users.Commands;

[Collection(ApplicationCollection.Name)]
public class CompleteEmailSignInTests(ApplicationFixture fixture) : FunctionalTest(fixture)
{
    [Fact]
    public async Task ShouldReturnTokenAndConfirmEmailForValidCode()
    {
        // Arrange
        var email = "user@spike.test";
        await SendAsync(new StartEmailSignInCommand(email));

        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand(email, EmailQueue.LastCodeSentTo(email)));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldNotBeNullOrWhiteSpace();

        (await Users.FindByEmailAsync(email))!.EmailConfirmed.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectWrongCode()
    {
        // Arrange
        var email = "user@spike.test";
        await SendAsync(new StartEmailSignInCommand(email));
        var wrongCode = EmailQueue.LastCodeSentTo(email) == "000000" ? "111111" : "000000";

        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand(email, wrongCode));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectReusedCode()
    {
        // Arrange
        var email = "user@spike.test";
        await SendAsync(new StartEmailSignInCommand(email));
        var code = EmailQueue.LastCodeSentTo(email);
        await SendAsync(new CompleteEmailSignInCommand(email, code));

        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand(email, code));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldLockOutAfterRepeatedWrongCodes()
    {
        // Arrange
        var email = "user@spike.test";
        await SendAsync(new StartEmailSignInCommand(email));
        var code = EmailQueue.LastCodeSentTo(email);
        var wrongCode = code == "000000" ? "111111" : "000000";

        for (var attempt = 0; attempt < 5; attempt++)
        {
            await SendAsync(new CompleteEmailSignInCommand(email, wrongCode));
        }

        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand(email, code));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectUnknownEmail()
    {
        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand("user@spike.test", "123456"));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("1234567")]
    [InlineData("abcdef")]
    public async Task ShouldRejectMalformedCode(string code)
    {
        // Act
        var result = await SendAsync(new CompleteEmailSignInCommand("user@spike.test", code));

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
