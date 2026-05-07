using CleanArchitecture.Application.ProjectTodoItems.Commands.AssignProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Commands.ChangeProjectTodoItemStatus;
using CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Commands.DeleteProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Commands.UpdateProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectKanbanBoard;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectTodoItems;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class ProjectTodoItems : IEndpointGroup
{
    public static string RoutePrefix => "/api/Projects/{projectId}/TodoItems";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetProjectTodoItems);
        groupBuilder.MapGet(GetProjectKanbanBoard, "/Kanban");
        groupBuilder.MapPost(CreateProjectTodoItem);
        groupBuilder.MapPut(UpdateProjectTodoItem, "{id}");
        groupBuilder.MapPut(AssignProjectTodoItem, "{id}/Assignee");
        groupBuilder.MapPut(ChangeProjectTodoItemStatus, "{id}/Status");
        groupBuilder.MapDelete(DeleteProjectTodoItem, "{id}");
    }

    public static async Task<Ok<IReadOnlyCollection<ProjectTodoItemDto>>> GetProjectTodoItems(ISender sender, int projectId)
    {
        var result = await sender.Send(new GetProjectTodoItemsQuery(projectId));

        return TypedResults.Ok(result);
    }

    public static async Task<Ok<ProjectKanbanBoardVm>> GetProjectKanbanBoard(ISender sender, int projectId)
    {
        var result = await sender.Send(new GetProjectKanbanBoardQuery(projectId));

        return TypedResults.Ok(result);
    }

    public static async Task<Results<Created<int>, BadRequest>> CreateProjectTodoItem(ISender sender, int projectId, CreateProjectTodoItemCommand command)
    {
        if (projectId != command.ProjectId)
        {
            return TypedResults.BadRequest();
        }

        var id = await sender.Send(command);

        return TypedResults.Created($"/api/Projects/{projectId}/TodoItems/{id}", id);
    }

    public static async Task<Results<NoContent, BadRequest>> UpdateProjectTodoItem(ISender sender, int projectId, int id, UpdateProjectTodoItemCommand command)
    {
        if (projectId != command.ProjectId || id != command.Id)
        {
            return TypedResults.BadRequest();
        }

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, BadRequest>> AssignProjectTodoItem(ISender sender, int projectId, int id, AssignProjectTodoItemCommand command)
    {
        if (projectId != command.ProjectId || id != command.Id)
        {
            return TypedResults.BadRequest();
        }

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, BadRequest>> ChangeProjectTodoItemStatus(ISender sender, int projectId, int id, ChangeProjectTodoItemStatusCommand command)
    {
        if (projectId != command.ProjectId || id != command.Id)
        {
            return TypedResults.BadRequest();
        }

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public static async Task<NoContent> DeleteProjectTodoItem(ISender sender, int projectId, int id)
    {
        await sender.Send(new DeleteProjectTodoItemCommand(projectId, id));

        return TypedResults.NoContent();
    }
}
