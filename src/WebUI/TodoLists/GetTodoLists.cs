using CleanArchitecture.Application.TodoLists.Queries.GetTodos;

namespace CleanArchitecture.WebUI.TodoLists;

public class GetTodoLists : EndPointBase
{
    public override void Map(WebApplication app)
    {
        MapGet(app,
             async (ISender sender) => await sender.Send(new GetTodosQuery()));
    }
}
