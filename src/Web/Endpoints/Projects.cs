using CleanArchitecture.Application.Projects.Commands.CreateProject;
using CleanArchitecture.Application.Projects.Queries.GetProjects;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Projects : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetProjects);
        groupBuilder.MapPost(CreateProject);
    }

    public static async Task<Ok<List<ProjectDto>>> GetProjects(ISender sender)
    {
        var result = await sender.Send(new GetProjectsQuery());

        return TypedResults.Ok(result);
    }

    public static async Task<Created<int>> CreateProject(ISender sender, CreateProjectCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/api/Projects/{id}", id);
    }
}
