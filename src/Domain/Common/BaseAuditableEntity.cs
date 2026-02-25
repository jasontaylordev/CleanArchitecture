namespace CleanArchitecture.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity, IDatetimeAuditable, IUserAuditable
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
