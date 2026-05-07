using CleanArchitecture.Application.ProjectTodoStatuses.Queries.GetProjectTodoStatuses;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class ProjectTodoStatuses : IEndpointGroup
{
    public static string RoutePrefix => "/api/Projects/{projectId}/TodoStatuses";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetProjectTodoStatuses);
    }

    public static async Task<Ok<IReadOnlyCollection<ProjectTodoStatusDto>>> GetProjectTodoStatuses(ISender sender, int projectId)
    {
        var result = await sender.Send(new GetProjectTodoStatusesQuery(projectId));

        return TypedResults.Ok(result);
    }
}
