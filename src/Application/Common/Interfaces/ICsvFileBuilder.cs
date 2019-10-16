using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile;
using System.Collections.Generic;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemFileRecord> records);
    }
}
