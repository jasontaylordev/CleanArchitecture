using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;

namespace CleanArchitecture.WebUI.Endpoints;

public class TodoListsEndpointGroup : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        MapGroup("TodoLists", app);

        MapGet("GetTodoLists",
            async (ISender sender) => await sender.Send(new GetTodosQuery()));

        MapGet("ExportTodos", "Export/{id}",
            async (ISender sender, int id) =>
            {
                var vm = await sender.Send(new ExportTodosQuery { ListId = id });
                return Results.File(vm.Content, vm.ContentType, vm.FileName);
            });

        MapPost("CreateTodoList",
            async (ISender sender, CreateTodoListCommand command) => await sender.Send(command));

        MapPut("UpdateTodoList", "{id}",
                async (ISender sender, int id, UpdateTodoListCommand command) =>
                {
                    if (id != command.Id) return Results.BadRequest();
                    await sender.Send(command);
                    return Results.NoContent();
                });

        MapDelete("DeleteTodoList", "{id}",
            async (ISender sender, int id) =>
            {
                await sender.Send(new DeleteTodoListCommand(id));
                return Results.NoContent();
            });
    }
}
