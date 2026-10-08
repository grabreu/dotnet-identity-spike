using Spike.Application.Users.Commands.CompleteOnboarding;

namespace Spike.Application.FunctionalTests.Users.Commands;

[Collection(ApplicationCollection.Name)]
public class CompleteOnboardingTests(ApplicationFixture fixture) : FunctionalTest(fixture)
{
    [Fact]
    public async Task ShouldStoreTrimmedDisplayNameAndMarkUserAsOnboarded()
    {
        // Arrange
        var email = "user@spike.test";
        var user = await Users.CreateAsync(email);

        // Act
        var result = await SendAsync(new CompleteOnboardingCommand(user.Id, "  Ada Lovelace  "));

        // Assert
        result.IsSuccess.ShouldBeTrue();

        var updated = await Users.FindByEmailAsync(email);
        updated!.DisplayName.ShouldBe("Ada Lovelace");
        updated.OnboardedAt.ShouldNotBeNull();
    }

    [Fact]
    public async Task ShouldRejectSecondOnboarding()
    {
        // Arrange
        var user = await Users.CreateAsync("user@spike.test");
        await SendAsync(new CompleteOnboardingCommand(user.Id, "Ada"));

        // Act
        var result = await SendAsync(new CompleteOnboardingCommand(user.Id, "Grace"));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownUser()
    {
        // Act
        var result = await SendAsync(new CompleteOnboardingCommand(Guid.NewGuid(), "Ada"));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ShouldRejectBlankDisplayName(string displayName)
    {
        // Arrange
        var user = await Users.CreateAsync("user@spike.test");

        // Act
        var result = await SendAsync(new CompleteOnboardingCommand(user.Id, displayName));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectDisplayNameLongerThanLimit()
    {
        // Arrange
        var user = await Users.CreateAsync("user@spike.test");

        // Act
        var result = await SendAsync(new CompleteOnboardingCommand(user.Id, new string('a', 101)));

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
