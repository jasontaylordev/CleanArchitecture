using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;

namespace CleanArchitecture.WebUI.TodoItems;

public class DeleteTodoItem : EndPointBase
{
    public override void Map(WebApplication app)
    {
        MapDelete(app, "{id}",
            async (ISender sender, int id) =>
            {
                await sender.Send(new DeleteTodoItemCommand(id));
                return Results.NoContent();
            });
    }
}
