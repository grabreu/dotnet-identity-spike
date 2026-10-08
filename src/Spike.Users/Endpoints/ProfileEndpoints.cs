using Spike.Users.Commands.CompleteOnboarding;
using Spike.Users.Queries.GetCurrentUser;

namespace Spike.Users.Endpoints;

public record CompleteOnboardingRequest(string DisplayName);

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .WithTags("Users");

        group
            .MapGet("/me", GetCurrentUserAsync)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Get Current User")
            .WithDescription("Retrieves the currently authenticated user.")
            .Produces<CurrentUserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group
            .MapPost("/me/onboarding", CompleteOnboardingAsync)
            .RequireAuthorization()
            .WithName("CompleteOnboarding")
            .WithSummary("Complete Onboarding")
            .WithDescription("Completes the onboarding process for the current user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        return endpoints;
    }

    private static ValueTask<IResult> GetCurrentUserAsync(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return sender.Send(new GetCurrentUserQuery(userId), cancellationToken).ToOk();
    }

    private static ValueTask<IResult> CompleteOnboardingAsync(CompleteOnboardingRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return sender.Send(new CompleteOnboardingCommand(userId, request.DisplayName), cancellationToken).ToNoContent();
    }
}
