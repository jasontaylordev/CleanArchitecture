using CleanArchitecture.Application.TodoLists.Commands.DeleteTodoList;

namespace CleanArchitecture.WebUI.TodoLists;

public class DeleteTodoList : EndPointBase
{
    public override void Map(WebApplication app)
    {
        MapDelete(app, "{id}",
            async (ISender sender, int id) =>
            {
                await sender.Send(new DeleteTodoListCommand(id));
                return Results.NoContent();
            });
    }
}
