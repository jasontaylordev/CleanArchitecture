namespace CleanArchitecture.Domain.Events;

public class UserCreatedEvent(string displayName, string? email) : BaseEvent
{
    public string DisplayName => displayName;
    public string? Email => email;
}
