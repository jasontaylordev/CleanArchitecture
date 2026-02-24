namespace CleanArchitecture.Domain.Interfaces;

public interface IUserAuditable
{
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
}
