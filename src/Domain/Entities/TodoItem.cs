using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Domain.Events;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class TodoItem : AuditableEntity, IHasDomainEvent
    {
        private bool _done;

        public TodoItem()
        {
            DomainEvents = new List<DomainEvent>();
        }

        public List<DomainEvent> DomainEvents { get; set; }

        public bool Done
        {
            get => _done;
            set
            {
                if (value == true && _done == false)
                {
                    DomainEvents.Add(new TodoItemCompletedEvent(this));
                }
                _done = value;
            }
        }

        public int Id { get; set; }

        public TodoList List { get; set; }

        public int ListId { get; set; }

        public string Note { get; set; }

        public PriorityLevel Priority { get; set; }

        public DateTime? Reminder { get; set; }

        public string Title { get; set; }
    }
}