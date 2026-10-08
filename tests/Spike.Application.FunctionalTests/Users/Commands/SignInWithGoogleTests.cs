using Spike.Application.Common.Identity;
using Spike.Application.Users.Commands.SignInWithGoogle;

namespace Spike.Application.FunctionalTests.Users.Commands;

[Collection(ApplicationCollection.Name)]
public class SignInWithGoogleTests(ApplicationFixture fixture) : FunctionalTest(fixture)
{
    [Fact]
    public async Task ShouldRejectInvalidToken()
    {
        // Act
        var result = await SendAsync(new SignInWithGoogleCommand("unknown-token"));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectEmptyToken()
    {
        // Act
        var result = await SendAsync(new SignInWithGoogleCommand(""));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldCreateConfirmedUserLinkedToGoogleForNewEmail()
    {
        // Arrange
        var email = "user@spike.test";
        GoogleTokenValidator.Accept("token", new GoogleUserInfo(Guid.NewGuid().ToString(), email));

        // Act
        var result = await SendAsync(new SignInWithGoogleCommand("token"));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.AccessToken.ShouldNotBeNullOrWhiteSpace();

        var user = await Users.FindByEmailAsync(email);
        user.ShouldNotBeNull();
        user.EmailConfirmed.ShouldBeTrue();

        (await Users.GetLoginsAsync(user)).ShouldHaveSingleItem().LoginProvider.ShouldBe("Google");
    }

    [Fact]
    public async Task ShouldLinkAndConfirmExistingUserWithSameEmail()
    {
        // Arrange
        var email = "user@spike.test";
        var existing = await Users.CreateAsync(email);
        GoogleTokenValidator.Accept("token", new GoogleUserInfo(Guid.NewGuid().ToString(), email));

        // Act
        var result = await SendAsync(new SignInWithGoogleCommand("token"));

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var user = await Users.FindByEmailAsync(email);
        user!.Id.ShouldBe(existing.Id);
        user.EmailConfirmed.ShouldBeTrue();

        (await Users.GetLoginsAsync(user)).ShouldHaveSingleItem();
    }

    [Fact]
    public async Task ShouldReuseUserOnRepeatedSignIn()
    {
        // Arrange
        var email = "user@spike.test";
        GoogleTokenValidator.Accept("token", new GoogleUserInfo(Guid.NewGuid().ToString(), email));
        await SendAsync(new SignInWithGoogleCommand("token"));

        // Act
        var result = await SendAsync(new SignInWithGoogleCommand("token"));

        // Assert
        result.IsSuccess.ShouldBeTrue();

        (await Users.GetLoginsAsync((await Users.FindByEmailAsync(email))!)).ShouldHaveSingleItem();
    }
}
