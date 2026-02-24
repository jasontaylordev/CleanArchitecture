namespace CleanArchitecture.Domain.Interfaces;

public interface IDateAuditable
{
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
