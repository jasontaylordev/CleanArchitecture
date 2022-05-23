using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Persistence.Interceptors;

public class EventSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly List<BaseEntity> _entitiesWithEvents = new();

    public EventSaveChangesInterceptor(IDomainEventDispatcher domainEventDispatcher)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StoreEvents(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        StoreEvents(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StoreEvents(DbContext? context)
    {
        if (context == null) return;

        _entitiesWithEvents.AddRange(
            context.ChangeTracker.Entries<BaseEntity>()
               .Select(e => e.Entity)
               .Where(e => e.DomainEvents.Any())
               .ToArray());
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        DispatchEvents();

        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        DispatchEvents();

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void DispatchEvents()
    {
        if (!_entitiesWithEvents.Any()) return;

        _domainEventDispatcher.DispatchEvents(_entitiesWithEvents);

        _entitiesWithEvents.Clear();
    }
}
