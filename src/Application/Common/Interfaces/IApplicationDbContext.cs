using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }

    DbSet<TodoItem> TodoItems { get; }

    DbSet<Project> Projects { get; }

    DbSet<ProjectTodoItem> ProjectTodoItems { get; }

    DbSet<ProjectTodoStatus> ProjectTodoStatuses { get; }

    DbSet<UserNotification> UserNotifications { get; }

    DbSet<EmailOutboxMessage> EmailOutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
