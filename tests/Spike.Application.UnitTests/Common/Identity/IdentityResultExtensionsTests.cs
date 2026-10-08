using Desfecho;
using Microsoft.AspNetCore.Identity;
using Spike.Application.Common.Identity;

namespace Spike.Application.UnitTests.Common.Identity;

public class IdentityResultExtensionsTests
{
    [Fact]
    public void EachIdentityErrorBecomesAValidationErrorKeyedByItsCode()
    {
        var identityResult = IdentityResult.Failed(
            new IdentityError { Code = "DuplicateEmail", Description = "Email is already taken." },
            new IdentityError { Code = "InvalidEmail", Description = "Email is invalid." });

        var errors = identityResult.ToErrors().Items;

        errors.Count.ShouldBe(2);
        errors.ShouldAllBe(error => error.Type == ErrorType.Validation);
        errors.Select(error => error.Property).ShouldBe(["DuplicateEmail", "InvalidEmail"]);
        errors.Select(error => error.Description).ShouldBe(["Email is already taken.", "Email is invalid."]);
    }

    [Fact]
    public void FailedResultWithoutErrorsBecomesASingleFailure()
    {
        var identityResult = IdentityResult.Failed();

        var errors = identityResult.ToErrors().Items;

        var error = errors.Single();
        error.Type.ShouldBe(ErrorType.Failure);
        error.Description.ShouldBe("Identity operation failed.");
    }
}
