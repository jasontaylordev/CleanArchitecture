using CleanArchitecture.Domain.Common;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class TodoList : AuditableEntity
    {
        public int Id { get; private set; }

        public string Title { get; private set; }

        public string Colour { get; set; }

        public IList<TodoItem> Items { get; } = new List<TodoItem>();


        /// <summary>
        /// For entity framework use.
        /// </summary>
        private TodoList()
        {
        }

        public TodoList(string title)
        {
            Title = title;
        }

        public void ChangeTitle(string newTitle)
        {
            Title = newTitle;
        }
    }
}
