using System.Security.Claims;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Endpoints;

public class Users : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();
        groupBuilder.MapGet("profile", GetProfile).RequireAuthorization();
        groupBuilder.MapPost(Logout, "logout").RequireAuthorization();
    }

    [EndpointSummary("Get current user profile")]
    [EndpointDescription("Returns the authenticated user's identifier, email claim, and roles.")]
    public static Task<Ok<UserProfileDto>> GetProfile(ClaimsPrincipal user)
    {
        return Task.FromResult(TypedResults.Ok(new UserProfileDto
        {
            Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = user.FindFirstValue(ClaimTypes.Email),
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        }));
    }

    [EndpointSummary("Log out")]
    [EndpointDescription("Logs out the current user by clearing the authentication cookie.")]
    public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(SignInManager<ApplicationUser> signInManager, [FromBody] object empty)
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return TypedResults.Ok();
        }

        return TypedResults.Unauthorized();
    }
}

public class UserProfileDto
{
    public string? Id { get; init; }
    public string? Email { get; init; }
    public List<string> Roles { get; init; } = [];
}
