using CleanArchitecture.Application.TodoLists.Commands.CreateTodoList;

namespace CleanArchitecture.WebUI.TodoLists;

public class CreateTodoList : EndPointBase
{
    public override void Map(WebApplication app)
    {
        MapPost(app,
            async (ISender sender, CreateTodoListCommand command) => await sender.Send(command));
    }
}
