namespace CleanArchitecture.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
        int? LocationHeaderId { get; }
        int? LocationId { get; }
        string Schema { get; }
}
