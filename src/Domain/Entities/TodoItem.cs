using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities
{
    public class TodoItem : AuditableEntity
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsComplete { get; set; }
    }
}
