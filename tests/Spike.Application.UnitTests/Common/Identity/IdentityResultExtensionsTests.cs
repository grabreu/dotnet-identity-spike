using Desfecho;
using Microsoft.AspNetCore.Identity;
using Spike.Application.Common.Identity;

namespace Spike.Application.UnitTests.Common.Identity;

public class IdentityResultExtensionsTests
{
    [Fact]
    public void EachIdentityErrorBecomesAValidationErrorKeyedByItsCode()
    {
        // Arrange
        var identityResult = IdentityResult.Failed(
            new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." },
            new IdentityError { Code = "InvalidEmail", Description = "Email is invalid." });

        // Act
        var errors = identityResult.ToErrors().Items;

        // Assert
        errors.Count.ShouldBe(2);
        errors.ShouldAllBe(error => error.Type == ErrorType.Validation);
        errors.Select(error => error.Property).ShouldBe(["DuplicateEmail", "InvalidEmail"]);
        errors.Select(error => error.Description).ShouldBe(["Email is already taken.", "Email is invalid."]);
    }

    [Fact]
    public void FailedResultWithoutErrorsBecomesASingleFailure()
    {
        // Arrange
        var identityResult = IdentityResult.Failed();

        // Act
        var errors = identityResult.ToErrors().Items;

        // Assert
        var error = errors.Single();
        error.Type.ShouldBe(ErrorType.Failure);
        error.Description.ShouldBe("Identity operation failed.");
    }
}
