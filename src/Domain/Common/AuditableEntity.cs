using System;
using System.Collections.Generic;

namespace CleanArchitecture.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public List<DomainEvent> Events { get; } = new List<DomainEvent>();

        public DateTime? LastModified { get; set; }

        public string LastModifiedBy { get; set; }
    }
}