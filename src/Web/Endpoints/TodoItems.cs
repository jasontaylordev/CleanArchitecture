using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class TodoItems : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapPost(CreateTodoItem);
        groupBuilder.MapPut(UpdateTodoItem, "{id}");
        groupBuilder.MapPatch(UpdateTodoItemDetail, "UpdateDetail/{id}");
        groupBuilder.MapDelete(DeleteTodoItem, "{id}");
    }

    [EndpointSummary("Create a new Todo Item")]
    [EndpointDescription("Creates a new todo item using the provided details and returns the ID of the created item.")]
    public static async Task<Created<int>> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoItems)}/{id}", id);
    }

    [EndpointSummary("Update a Todo Item")]
    [EndpointDescription("Updates the specified todo item. The ID in the URL must match the ID in the payload.")]
    public static async Task<Results<NoContent, BadRequest>> UpdateTodoItem(ISender sender, int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
            return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    [EndpointSummary("Update Todo Item Details")]
    [EndpointDescription("Updates the detail fields of a specific todo item. The ID in the URL must match the ID in the payload.")]
    public static async Task<Results<NoContent, BadRequest>> UpdateTodoItemDetail(ISender sender, int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id) return TypedResults.BadRequest();

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    [EndpointSummary("Delete a Todo Item")]
    [EndpointDescription("Deletes the todo item with the specified ID.")]
    public static async Task<NoContent> DeleteTodoItem(ISender sender, int id)
    {
        await sender.Send(new DeleteTodoItemCommand(id));

        return TypedResults.NoContent();
    }
}
