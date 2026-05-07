namespace CleanArchitecture.Domain.Entities;

public class UserNotification : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string LinkUrl { get; set; } = string.Empty;

    public bool IsRead { get; set; }
}
