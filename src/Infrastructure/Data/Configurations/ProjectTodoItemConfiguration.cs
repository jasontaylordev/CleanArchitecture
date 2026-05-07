using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class ProjectTodoItemConfiguration : IEntityTypeConfiguration<ProjectTodoItem>
{
    public void Configure(EntityTypeBuilder<ProjectTodoItem> builder)
    {
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.ReporterUserId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(t => t.AssigneeUserId)
            .HasMaxLength(450);

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => new { t.ProjectId, t.StatusId });
        builder.HasIndex(t => t.AssigneeUserId);
        builder.HasIndex(t => t.ReporterUserId);
        builder.HasIndex(t => t.DueDate);

        builder.HasOne(t => t.Project)
            .WithMany(p => p.TodoItems)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Status)
            .WithMany(s => s.TodoItems)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
