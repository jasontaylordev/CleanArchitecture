namespace CleanArchitecture.Domain.Entities;

public class EmailOutboxMessage : BaseAuditableEntity
{
    public string To { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public DateTimeOffset? SentAt { get; set; }

    public string Status { get; set; } = "Pending";
}
