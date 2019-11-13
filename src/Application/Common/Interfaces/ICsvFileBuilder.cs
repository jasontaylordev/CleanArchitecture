using CleanArchitecture.Application.TodoLists.Queries.ExportTodos;
using System.Collections.Generic;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
    }
}
