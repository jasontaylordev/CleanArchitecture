using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace CleanArchitecture.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public TodoItems(ISender sender) : base(sender) { }

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoItemsWithPagination)
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}");
    }

    public async Task<PaginatedList<TodoItemBriefDto>> GetTodoItemsWithPagination([AsParameters] GetTodoItemsWithPaginationQuery query)
    {
        return await _sender.Send(query);
    }

    public async Task<int> CreateTodoItem(CreateTodoItemCommand command)
    {
        return await _sender.Send(command);
    }

    public async Task<IResult> UpdateTodoItem(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await _sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> UpdateTodoItemDetail(int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await _sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoItem(int id)
    {
        await _sender.Send(new DeleteTodoItemCommand(id));
        return Results.NoContent();
    }
}
