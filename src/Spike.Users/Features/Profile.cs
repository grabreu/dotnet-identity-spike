using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Spike.Users.Features;

public record ProfileResponse(string? Id, string? Email);

public static class ProfileEndpoint
{
    public static IEndpointRouteBuilder MapProfile(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users/me", (ClaimsPrincipal user) =>
        {
            return TypedResults.Ok(new ProfileResponse(user.FindFirstValue(ClaimTypes.NameIdentifier), user.FindFirstValue(ClaimTypes.Email)));
        })
        .WithName("Profile")
        .WithTags("Users")
        .WithSummary("Get profile")
        .WithDescription("Returns the authenticated user's profile.")
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();

        return app;
    }
}
