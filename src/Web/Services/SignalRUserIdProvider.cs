using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace CleanArchitecture.Web.Services;

public sealed class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
