using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile;
using CsvHelper.Configuration;

namespace CleanArchitecture.Infrastructure.Files.Maps
{
    public class TodoItemFileRecordMap : ClassMap<TodoItemFileRecord>
    {
        public TodoItemFileRecordMap()
        {
            AutoMap();
            Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
        }
    }
}
