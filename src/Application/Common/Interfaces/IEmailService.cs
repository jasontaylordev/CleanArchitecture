namespace CleanArchitecture.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toUserId, string subject, string body, CancellationToken cancellationToken);
}
