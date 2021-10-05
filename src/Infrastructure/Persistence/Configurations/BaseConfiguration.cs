using CleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : CommonEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
           // builder.Property<int>(x => x.Id).HasColumnName(@"Id").IsRequired(true).UseIdentityColumn().ValueGeneratedOnAdd();

            builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        }
    }
}
