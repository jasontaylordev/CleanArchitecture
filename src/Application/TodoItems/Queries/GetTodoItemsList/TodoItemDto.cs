using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList
{
        public class TodoItemDto : IMapFrom<TodoItem>
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public bool IsComplete { get; set; }
        }
}
