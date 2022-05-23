using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArchitecture.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    [NotMapped]
    public List<BaseEvent> DomainEvents { get; set; } = new();
}
