using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;

namespace CleanArchitecture.WebUI.TodoItems;

public class UpdateTodoItemDetail : AbstractEndpoint
{
    public override void Map(WebApplication app)
    {
        MapPut(app, "UpdateDetail/{id}",
            async (ISender sender, int id, UpdateTodoItemDetailCommand command) =>
            {
                if (id != command.Id) return Results.BadRequest();

                await sender.Send(command);

                return Results.NoContent();
            });
    }
}
