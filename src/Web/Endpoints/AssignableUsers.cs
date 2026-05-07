using CleanArchitecture.Application.Users.Queries.GetAssignableUsers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class AssignableUsers : IEndpointGroup
{
    public static string RoutePrefix => "/api/Users/Assignable";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetAssignableUsers);
    }

    public static async Task<Ok<IReadOnlyCollection<AssignableUserDto>>> GetAssignableUsers(ISender sender)
    {
        var result = await sender.Send(new GetAssignableUsersQuery());

        return TypedResults.Ok(result);
    }
}
