using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoLists)
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }

    public async Task<Ok<TodosVm>> GetTodoLists(IMediator sender, CancellationToken cancellationToken)
    {
        var vm = await sender.Send(new GetTodosQuery(), cancellationToken);

        return TypedResults.Ok(vm);
    }

    public async Task<Created<int>> CreateTodoList(IMediator sender, CreateTodoListCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"/{nameof(TodoLists)}/{id}", id);
    }

    public async Task<Results<NoContent, BadRequest>> UpdateTodoList(IMediator sender, int id, UpdateTodoListCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return TypedResults.BadRequest();
        
        await sender.Send(command, cancellationToken);

        return TypedResults.NoContent();
    }

    public async Task<NoContent> DeleteTodoList(IMediator sender, int id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteTodoListCommand(id), cancellationToken);

        return TypedResults.NoContent();
    }
}
