using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItem
{
    public class TodoItemVm : IMapFrom<TodoItem>
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsComplete { get; set; }
    }
}
