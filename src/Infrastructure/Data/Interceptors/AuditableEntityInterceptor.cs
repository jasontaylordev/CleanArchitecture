using System.Text.Json;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CleanArchitecture.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IUser _user;
    private readonly TimeProvider _dateTime;

    public AuditableEntityInterceptor(
        IUser user,
        TimeProvider dateTime)
    {
        _user = user;
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        var audits = new List<AuditLog>();
        
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                var utcNow = _dateTime.GetUtcNow();
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _user.Id;
                    entry.Entity.Created = utcNow;
                } 
                entry.Entity.LastModifiedBy = _user.Id;
                entry.Entity.LastModified = utcNow;
                
                var action = entry.State == EntityState.Added ? AuditAction.Create : AuditAction.Update;
                
                if (entry.HasChangedOwnedEntities() && entry.State == EntityState.Unchanged)
                    action = AuditAction.Update;
                
                var audit = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = GetPrimaryKey(entry) ?? string.Empty,
                    Action = action,
                    UserId = _user.Id
                };

                // Capture old and new values for updates
                if (action == AuditAction.Update)
                {
                    var changes = GetChangedProperties(entry);
                    audit.OldValues = changes.OldValues;
                    audit.NewValues = changes.NewValues;
                    audit.ChangedColumns = changes.ChangedColumns;
                }
                else if (action == AuditAction.Create)
                {
                    audit.NewValues = SerializeCurrentEntity(entry);
                }
                
                audits.Add(audit);
            }
        }
        
        if (audits.Count > 0)
        {
            if (context is ApplicationDbContext appDb)
            {
                appDb.AuditLog.AddRange(audits);
            }
        }
    }
    
    private static string? GetPrimaryKey(EntityEntry entry)
    {
        var keyProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
        var value = keyProperty?.CurrentValue ?? keyProperty?.OriginalValue;
        return value?.ToString();
    }

    private static string? SerializeCurrentEntity(EntityEntry entry)
    {
        var currentValues = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;

            currentValues[propertyName] = property.CurrentValue;
        }

        foreach (var reference in entry.References.Where(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned()))
        {
            var ownedEntry = reference.TargetEntry!;
            var propertyName = reference.Metadata.Name;

            foreach (var property in ownedEntry.Properties)
            {
                var fullPropertyName = $"{propertyName}.{property.Metadata.Name}";
                currentValues[fullPropertyName] = property.CurrentValue;
            }
        }

        if (currentValues.Count == 0)
            return null;

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return JsonSerializer.Serialize(currentValues, jsonOptions);
    }

    private static (string? OldValues, string? NewValues, string? ChangedColumns) GetChangedProperties(EntityEntry entry)
    {
        var oldValues = new Dictionary<string, object?>();
        var newValues = new Dictionary<string, object?>();
        var changedColumns = new List<string>();

        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;

            oldValues[propertyName] = property.OriginalValue;
            newValues[propertyName] = property.CurrentValue;

            if (property.IsModified)
            {
                changedColumns.Add(propertyName);
            }
        }

        foreach (var reference in entry.References.Where(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned()))
        {
            var ownedEntry = reference.TargetEntry!;
            var propertyName = reference.Metadata.Name;

            foreach (var property in ownedEntry.Properties)
            {
                var fullPropertyName = $"{propertyName}.{property.Metadata.Name}";
                
                oldValues[fullPropertyName] = property.OriginalValue;
                newValues[fullPropertyName] = property.CurrentValue;

                if (property.IsModified || ownedEntry.State == EntityState.Added || ownedEntry.State == EntityState.Modified)
                {
                    changedColumns.Add(fullPropertyName);
                }
            }
        }

        if (changedColumns.Count == 0)
            return (null, null, null);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        return (
            JsonSerializer.Serialize(oldValues, jsonOptions),
            JsonSerializer.Serialize(newValues, jsonOptions),
            string.Join(",", changedColumns)
        );
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned() && 
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
