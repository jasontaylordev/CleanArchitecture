using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class TodoItem : AuditableEntity, IHasDomainEvent
    {
        public int Id { get; set; }

        public TodoList List { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public PriorityLevel Priority { get; private set; } = PriorityLevel.None;

        public DateTime? Reminder { get; set; }

        private bool _done;
        public bool Done
        {
            get => _done;
            set
            {
                if (value && _done == false)
                {
                    DomainEvents.Add(new TodoItemCompletedEvent(this));
                }

                _done = value;
            }
        }

        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

        /// <summary>
        /// For entity framework use.
        /// </summary>
        private TodoItem()
        {
        }

        public TodoItem(int listId, string title, bool isDone = false)
        {
            ListId = listId;
            Title = title;
            Done = isDone;
        }

        public bool ChangePriority(PriorityLevel level)
        {
            if (Priority == level)
            {
                return false;
            }

            Priority = level;
            return true;
        }
    }
}
