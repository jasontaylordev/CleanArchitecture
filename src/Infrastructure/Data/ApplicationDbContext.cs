using System.Linq.Expressions;
using System.Reflection;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        foreach (var entityType in builder.Model.GetEntityTypes()
                                            .Where(t => typeof(ISoftDelete)
                                            .IsAssignableFrom(t.ClrType)))
        {
            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var deleted = Expression.Property(parameter, nameof(ISoftDelete.Deleted));
            var hasValue = Expression.Property(deleted, nameof(Nullable<DateTimeOffset>.HasValue));
            var filter = Expression.Lambda(Expression.Not(hasValue), parameter);

            builder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }
    }
}
