using Cubido.Template.Application.Common.Models;
using Cubido.Template.Application.TodoItems.Commands.CreateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.DeleteTodoItem;
using Cubido.Template.Application.TodoItems.Commands.UpdateTodoItem;
using Cubido.Template.Application.TodoItems.Commands.UpdateTodoItemDetail;
using Cubido.Template.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Cubido.Template.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Sqiddler.AspNetCore;

namespace Cubido.Template.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder route)
    {
        route.AllowAnonymous()
            .MapGet(GetTodoItemsWithPagination)
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}")
            ;
    }

    public async Task<Ok<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination(ISender sender, [AsParameters] GetTodoItemsWithPaginationQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }

    public async Task<Created<int>> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        var id = await sender.Send(command);

        return TypedResults.Created($"/{nameof(TodoItems)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoItem(ISender sender, SqidParam<TodoItem> id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return TypedResults.BadRequest();
        }

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoItemDetail(ISender sender, SqidParam<TodoItem> id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
        {
            return TypedResults.BadRequest();
        }

        await sender.Send(command);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoItem(ISender sender, SqidParam<TodoItem> id)
    {
        await sender.Send(new DeleteTodoItemCommand() { Id = id });

        return TypedResults.NoContent();
    }
}
