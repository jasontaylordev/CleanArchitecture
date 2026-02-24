using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CleanArchitecture.Domain.Entities;

/// <summary>
/// User identity, where you can define more properties based on the business logic for your application.
/// You can also add <see cref="IHasDomainEvents"/> to perform the domain events based on the user creation or remove it.
/// In <see cref="IdentityUser{T}"/> You can change the Id type too, default is string.
/// </summary>
public class User : IdentityUser, IHasDomainEvents
{
    public string DisplayName { get; private set; } = null!;

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    private readonly List<BaseEvent> _domainEvents = [];
    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public User() { }
    public User(string displayName, string username)
    {
        DisplayName = displayName;
        UserName = username;
        Email = username;
    }

    public void AddDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
