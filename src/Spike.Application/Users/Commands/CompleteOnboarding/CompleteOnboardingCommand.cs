namespace Spike.Application.Users.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand(Guid UserId, string DisplayName) : ICommand<Result>;
