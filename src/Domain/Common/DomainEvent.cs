using MediatR;
using System;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Common
{
    public abstract class DomainEvent : INotification
    {
        public static Func<IMediator> Mediator { get; set; }

        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;

        public static async Task Raise<T>(T args) where T : INotification
        {
            IMediator mediator = Mediator.Invoke();
            await mediator.Publish<T>(args);
        }
    }
}