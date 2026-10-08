using Spike.Application.Users.Commands.CompleteOnboarding;
using Spike.Application.Users.Queries.GetCurrentUser;

namespace Spike.Application.FunctionalTests.Users.Queries;

[Collection(ApplicationCollection.Name)]
public class GetCurrentUserTests(ApplicationFixture fixture) : FunctionalTest(fixture)
{
    [Fact]
    public async Task ShouldReturnNotOnboardedUser()
    {
        // Arrange
        var email = "user@spike.test";
        var user = await Users.CreateAsync(email);

        // Act
        var result = await SendAsync(new GetCurrentUserQuery(user.Id));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Email.ShouldBe(email);
        result.Value.DisplayName.ShouldBeNull();
        result.Value.IsOnboarded.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldReturnOnboardedUser()
    {
        // Arrange
        var user = await Users.CreateAsync("user@spike.test");
        await SendAsync(new CompleteOnboardingCommand(user.Id, "Ada"));

        // Act
        var result = await SendAsync(new GetCurrentUserQuery(user.Id));

        // Assert
        result.Value.DisplayName.ShouldBe("Ada");
        result.Value.IsOnboarded.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldReturnNotFoundForUnknownUser()
    {
        // Act
        var result = await SendAsync(new GetCurrentUserQuery(Guid.NewGuid()));

        // Assert
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task ShouldRejectEmptyUserId()
    {
        // Act
        var result = await SendAsync(new GetCurrentUserQuery(Guid.Empty));

        // Assert
        result.IsError.ShouldBeTrue();
    }
}
