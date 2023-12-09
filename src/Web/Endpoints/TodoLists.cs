using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;

namespace CleanArchitecture.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public TodoLists(ISender sender) : base(sender) { }

    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoLists)
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }

    public async Task<TodosVm> GetTodoLists()
    {
        return await _sender.Send(new GetTodosQuery());
    }

    public async Task<int> CreateTodoList(CreateTodoListCommand command)
    {
        return await _sender.Send(command);
    }

    public async Task<IResult> UpdateTodoList(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await _sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoList(int id)
    {
        await _sender.Send(new DeleteTodoListCommand(id));
        return Results.NoContent();
    }
}
