using CleanArchitecture.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; set; }

    DbSet<TodoItem> TodoItems { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
