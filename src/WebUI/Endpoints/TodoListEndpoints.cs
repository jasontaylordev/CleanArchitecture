using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;
using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;
using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;
using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CleanArchitecture.Application.TodoLists.Queries.GetTodos;
using CleanArchitecture.WebUI.Filters;

namespace CleanArchitecture.WebUI.Endpoints;

public class TodoListEndpoints
{
    private const string GroupName = "TodoLists";

    public static void Map(WebApplication app)
    {
        var group = app
            .MapGroup($"/api/{GroupName}")
            .WithTags(GroupName)
            .RequireAuthorization()
            .WithOpenApi()
            .AddEndpointFilter<ApiExceptionFilter>();

        group
            .MapGet("/", async (ISender sender) => await sender.Send(new GetTodosQuery()))
            .WithName($"{GroupName}_GetTodoLists");

        group
            .MapGet("/Export/{id}",
                 async (ISender sender, int id) =>
                 {
                     var vm = await sender.Send(new ExportTodosQuery { ListId = id });
                     return Results.File(vm.Content, vm.ContentType, vm.FileName);
                 })
            .WithName($"{GroupName}_ExportTodos"); ;

        group
            .MapPost("/", async (ISender sender, CreateTodoListCommand command) => await sender.Send(command))
            .WithName($"{GroupName}_CreateTodoList"); ;

        group
            .MapPut("/{id}",
                async (ISender sender, int id, UpdateTodoListCommand command) =>
                {
                    if (id != command.Id) return Results.BadRequest();
                    await sender.Send(command);
                    return Results.NoContent();
                })
            .WithName($"{GroupName}_UpdateTodoList"); ;

        group
            .MapDelete("/{id}",
                async (ISender sender, int id) =>
                {
                    await sender.Send(new DeleteTodoListCommand(id));
                    return Results.NoContent();
                })
            .WithName($"{GroupName}_DeleteTodoList"); ;
    }
}
