namespace Spike.Application.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IQuery<Result<CurrentUserDto>>;
