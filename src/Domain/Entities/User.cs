using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// User identity entity, It has the <see cref="IHasDomainEvents"/> domain events for logs or other business logic to perform.
/// </summary>
public class User : IdentityUser, IHasDomainEvents
{
    public string DisplayName { get; private set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    private readonly List<BaseEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public User() { }
    public User(string displayName, string userName)
    {
        DisplayName = displayName;
        UserName = userName;
        Email = userName;
    }

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
