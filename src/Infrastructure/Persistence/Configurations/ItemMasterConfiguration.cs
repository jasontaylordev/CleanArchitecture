using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class ItemMasterConfiguration : IEntityTypeConfiguration<ItemMaster>
    {
        private readonly string? _schema;

        public ItemMasterConfiguration(string? schema)
        {
            _schema = schema;
        }
        public void Configure(EntityTypeBuilder<ItemMaster> entity)
        {
            if (!string.IsNullOrWhiteSpace(_schema))
                entity.ToTable("MAPF01", _schema);

            entity.HasKey(e => e.ItemNumber);

            entity.Property(e => e.ItemNumber)
                .IsRequired()
                .HasColumnName("ITM001")
                .HasColumnType("char(30)")
                .HasDefaultValueSql("' '")
                .HasConversion(Conversions.Trim);

            entity.Property(e => e.ItemTermType)
                .IsRequired()
                .HasColumnName("ITM002")
                .HasColumnType("char(2)")
                .HasDefaultValueSql("' '")
                .HasConversion(Conversions.Trim);

            entity.Property(e => e.ItemType)
                .IsRequired()
                .HasColumnName("ITM003")
                .HasColumnType("char(1)")
                .HasDefaultValueSql("' '")
                .HasConversion(Conversions.Trim);

            entity.Property(e => e.ItemDescription)
                .IsRequired()
                .HasColumnName("ITM004")
                .HasColumnType("char(50)")
                .HasDefaultValueSql("' '")
                .HasConversion(Conversions.Trim);

            entity.Property(e => e.ItemProdLine)
                .IsRequired()
                .HasColumnName("ITM005")
                .HasColumnType("numeric(3, 0)")
                .HasDefaultValueSql("0");

            entity.Property(e => e.ItemWeight)
                .IsRequired()
                .HasColumnName("ITM011")
                .HasColumnType("decimal(7, 3)")
                .HasDefaultValueSql("0");
        }
    }
}
