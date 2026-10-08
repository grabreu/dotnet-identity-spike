namespace Spike.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IQuery<Result<CurrentUserDto>>;
