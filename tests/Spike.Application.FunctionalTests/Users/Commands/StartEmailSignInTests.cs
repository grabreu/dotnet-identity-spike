using Spike.Application.Users.Commands.StartEmailSignIn;

namespace Spike.Application.FunctionalTests.Users.Commands;

[Collection(ApplicationCollection.Name)]
public class StartEmailSignInTests(ApplicationFixture fixture) : FunctionalTest(fixture)
{
    [Fact]
    public async Task ShouldCreateUserAndSendCodeForNewEmail()
    {
        // Arrange
        var email = "user@spike.test";

        // Act
        var result = await SendAsync(new StartEmailSignInCommand(email));

        // Assert
        result.IsSuccess.ShouldBeTrue();

        (await Users.FindByEmailAsync(email)).ShouldNotBeNull();

        EmailQueue.Messages.ShouldHaveSingleItem().To.ShouldBe(email);
        EmailQueue.LastCodeSentTo(email).Length.ShouldBe(6);
    }

    [Fact]
    public async Task ShouldSendCodeWithoutDuplicatingExistingUser()
    {
        // Arrange
        var email = "user@spike.test";
        await Users.CreateAsync(email);

        // Act
        var result = await SendAsync(new StartEmailSignInCommand(email));

        // Assert
        result.IsSuccess.ShouldBeTrue();

        EmailQueue.Messages.ShouldHaveSingleItem().To.ShouldBe(email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task ShouldRejectInvalidEmail(string email)
    {
        // Act
        var result = await SendAsync(new StartEmailSignInCommand(email));

        // Assert
        result.IsError.ShouldBeTrue();

        EmailQueue.Messages.ShouldBeEmpty();
    }
}
