namespace CleanArchitecture.Domain.Interfaces;

public interface IDatetimeAuditable
{
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
