using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Domain.Common;

public class BaseEvent : INotification
{
    public BaseEvent(object entity)
    {
        Entity = entity;
    }

    private object Entity { get; }

    public LogLevel Level { get; set; } = LogLevel.Information;
    public string Message { get; set; } = string.Empty;

    public string LogMessage => Entity.GetType().FullName + " " + Message;
}
