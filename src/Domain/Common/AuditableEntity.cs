using CleanArchitecture.Domain.Entities;
using System;

namespace CleanArchitecture.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }

        public string CreatedByUserId { get; set; }

        public DateTime? LastModified { get; set; }

        public string LastModifiedByUserId { get; set; }

        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser ModifiedByUser { get; set; }
    }
}
