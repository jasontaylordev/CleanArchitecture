using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;

namespace CleanArchitecture.WebUI.TodoLists;

public class ExportTodos : AbstractEndpoint
{
    public override void Map(WebApplication app)
    {
        MapGet(app, "Export/{id}",
             async (ISender sender, int id) => 
             {
                 var vm = await sender.Send(new ExportTodosQuery { ListId = id });
                 return Results.File(vm.Content, vm.ContentType, vm.FileName);
             });
    }
}