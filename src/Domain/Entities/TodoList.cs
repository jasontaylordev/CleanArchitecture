using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.ValueObjects;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class TodoList :  CommonEntity
    {
        public Colour Colour { get; set; } = Colour.White;

        public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
    }
}
