using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile;
using System.Collections.Generic;

namespace CleanArchitecture.Application
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemFileRecord> records);
    }
}
