using System.ComponentModel.DataAnnotations.Schema;

namespace Cubido.Template.Domain.Common;

public abstract class BaseEntity
{
    // This can easily be modified to be BaseEntity<T> and public T Id to support different key types.
    // Using non-generic integer types for simplicity
    public int Id { get; set; }

    private readonly List<BaseEvent> domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}
