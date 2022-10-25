using CleanArchitecture.Application.TodoLists.Commands.UpdateTodoList;

namespace CleanArchitecture.WebUI.TodoLists;

public class UpdateTodoList : AbstractEndpoint
{
    public override void Map(WebApplication app)
    {
        MapPut(app, "{id}",
            async (ISender sender, int id, UpdateTodoListCommand command) =>
            {
                if (id != command.Id) return Results.BadRequest();

                await sender.Send(command);

                return Results.NoContent();
            });
    }
}
