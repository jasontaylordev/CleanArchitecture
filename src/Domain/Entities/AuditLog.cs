namespace CleanArchitecture.Domain.Entities;

public class AuditLog : BaseEntity<Guid>
{
    public string EntityName { get; set; } = string.Empty;
    
    public string EntityId { get; set; } = string.Empty; // Id.ToString()
    
    public AuditAction Action { get; set; }
    
    public string? OldValues { get; set; }
    
    public string? NewValues { get; set; }
    
    public string? ChangedColumns { get; set; }
    
    public string? UserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
