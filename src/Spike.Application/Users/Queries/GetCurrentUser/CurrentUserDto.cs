namespace Spike.Application.Users.Queries.GetCurrentUser;

public record CurrentUserDto(Guid Id, string Email, string? DisplayName, bool IsOnboarded);
