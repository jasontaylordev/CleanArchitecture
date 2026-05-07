using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Data.Configurations;

public class EmailOutboxMessageConfiguration : IEntityTypeConfiguration<EmailOutboxMessage>
{
    public void Configure(EntityTypeBuilder<EmailOutboxMessage> builder)
    {
        builder.Property(t => t.To).HasMaxLength(320).IsRequired();
        builder.Property(t => t.Subject).HasMaxLength(300).IsRequired();
        builder.Property(t => t.Body).HasMaxLength(4000).IsRequired();
        builder.Property(t => t.Status).HasMaxLength(50).IsRequired();

        builder.HasIndex(t => new { t.Status, t.Created });
    }
}
