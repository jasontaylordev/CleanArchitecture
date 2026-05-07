using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class ProjectTodoStatusConfiguration : IEntityTypeConfiguration<ProjectTodoStatus>
{
    public void Configure(EntityTypeBuilder<ProjectTodoStatus> builder)
    {
        builder.Property(t => t.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(t => new { t.ProjectId, t.SortOrder });

        builder.HasOne(t => t.Project)
            .WithMany(p => p.Statuses)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
