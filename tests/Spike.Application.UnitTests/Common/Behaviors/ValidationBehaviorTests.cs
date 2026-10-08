using Desfecho;
using FluentValidation;
using Mediator;
using Spike.Application.Common.Behaviors;

namespace Spike.Application.UnitTests.Common.Behaviors;

public class ValidationBehaviorTests
{
    private record TestCommand(string Name, int Age) : ICommand<Result>;

    private record TestGenericCommand(string Name) : ICommand<Result<string>>;

    private static InlineValidator<TestCommand> NameRequiredValidator()
    {
        var validator = new InlineValidator<TestCommand>();
        validator.RuleFor(x => x.Name).NotEmpty();

        return validator;
    }

    [Fact]
    public async Task WithoutValidatorsCallsNextAndReturnsItsResponse()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestCommand, Result>([]);
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(new TestCommand("", 0), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        // Assert
        nextCalled.ShouldBeTrue();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ValidMessageCallsNextAndReturnsItsResponse()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestCommand, Result>([NameRequiredValidator()]);
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(new TestCommand("Ana", 30), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        // Assert
        nextCalled.ShouldBeTrue();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task InvalidMessageReturnsValidationErrorsWithoutCallingNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<TestCommand, Result>([NameRequiredValidator()]);
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(new TestCommand("", 30), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success());
        }, CancellationToken.None);

        // Assert
        nextCalled.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.Errors.Count.ShouldBe(1);
        result.Error.Type.ShouldBe(ErrorType.Validation);
        result.Error.Property.ShouldBe("Name");
    }

    [Fact]
    public async Task InvalidMessageReturnsValidationErrorsForGenericResult()
    {
        // Arrange
        var validator = new InlineValidator<TestGenericCommand>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var behavior = new ValidationBehavior<TestGenericCommand, Result<string>>([validator]);
        var nextCalled = false;

        // Act
        var result = await behavior.Handle(new TestGenericCommand(""), (_, _) =>
        {
            nextCalled = true;
            return ValueTask.FromResult(Result.Success("ok"));
        }, CancellationToken.None);

        // Assert
        nextCalled.ShouldBeFalse();
        result.IsError.ShouldBeTrue();
        result.Errors.Single().Property.ShouldBe("Name");
    }

    [Fact]
    public async Task FailuresFromAllValidatorsAreReported()
    {
        // Arrange
        var nameValidator = NameRequiredValidator();
        var ageValidator = new InlineValidator<TestCommand>();
        ageValidator.RuleFor(x => x.Age).GreaterThan(0);
        var behavior = new ValidationBehavior<TestCommand, Result>([nameValidator, ageValidator]);

        // Act
        var result = await behavior.Handle(new TestCommand("", 0), (_, _) => ValueTask.FromResult(Result.Success()), CancellationToken.None);

        // Assert
        result.IsError.ShouldBeTrue();
        result.Errors.Select(error => error.Property).ShouldBe(["Name", "Age"], ignoreOrder: true);
    }
}
