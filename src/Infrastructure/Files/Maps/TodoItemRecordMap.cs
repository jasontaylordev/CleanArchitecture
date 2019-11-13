using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using CsvHelper.Configuration;

namespace CleanArchitecture.Infrastructure.Files.Maps
{
    public class TodoItemRecordMap : ClassMap<TodoItemRecord>
    {
        public TodoItemRecordMap()
        {
            AutoMap();
            Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
        }
    }
}
