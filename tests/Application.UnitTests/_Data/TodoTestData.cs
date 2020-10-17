using System.Collections.Generic;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.UnitTests._Data
{
    public class TodoTestData
    {
        public static List<TodoList> TodoListData = new List<TodoList>
        {
            new TodoList { Id = 1, Title = "Project #1", Colour = "Red" },
            new TodoList { Id = 2, Title = "Project #2", Colour = "Green" },
        };

        public static List<TodoItem> TodoItemData = new List<TodoItem>
        {
            new TodoItem { Id = 11, ListId = 1, Title = "Item 11", Priority = PriorityLevel.Low },
            new TodoItem { Id = 12, ListId = 1, Title = "Item 12", Priority = PriorityLevel.Medium },

            new TodoItem { Id = 21, ListId = 2, Title = "Item 21", Priority = PriorityLevel.Medium },
            new TodoItem { Id = 22, ListId = 2, Title = "Item 22", Priority = PriorityLevel.High },
        };
    }
}
