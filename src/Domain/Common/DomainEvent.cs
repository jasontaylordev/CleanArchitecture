using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Common
{
    public interface IHasDomainEvent
    {
        public List<DomainEvent> DomainEvents { get; set; }
    }

    public abstract class DomainEvent
    {
        protected DomainEvent()
        {
            EventId = Guid.NewGuid();
            DateOccurred = DateTimeOffset.UtcNow;
        }

        public Guid EventId { get; }
        public DateTimeOffset DateOccurred { get; protected set; }
    }
}
