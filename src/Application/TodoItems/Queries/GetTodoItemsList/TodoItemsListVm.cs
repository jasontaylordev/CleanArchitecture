using System.Collections.Generic;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList
{
    public class TodoItemsListVm
    {
        public IList<TodoItemDto> TodoItems { get; set; }
    }
}
