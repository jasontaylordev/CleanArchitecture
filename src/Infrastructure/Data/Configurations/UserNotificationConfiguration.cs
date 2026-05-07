using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.Property(t => t.UserId).HasMaxLength(450).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Message).HasMaxLength(1000).IsRequired();
        builder.Property(t => t.LinkUrl).HasMaxLength(500).IsRequired();

        builder.HasIndex(t => new { t.UserId, t.IsRead });
    }
}
