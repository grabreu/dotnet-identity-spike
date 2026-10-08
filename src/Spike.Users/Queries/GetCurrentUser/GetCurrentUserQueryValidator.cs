namespace Spike.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
