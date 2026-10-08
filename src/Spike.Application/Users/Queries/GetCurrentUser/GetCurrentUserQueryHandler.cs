using Spike.Application.Common.Identity;

namespace Spike.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager) : IQueryHandler<GetCurrentUserQuery, Result<CurrentUserDto>>
{
    public async ValueTask<Result<CurrentUserDto>> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(query.UserId.ToString());

        if (user is null)
        {
            return Result.NotFound("User not found.");
        }

        return new CurrentUserDto(user.Id, user.Email!, user.DisplayName, user.OnboardedAt is not null);
    }
}
