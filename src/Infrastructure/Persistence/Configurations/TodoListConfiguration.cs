using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class TodoListConfiguration : BaseConfiguration<TodoList>
    {
        public override void Configure(EntityTypeBuilder<TodoList> builder)
        {
            builder.OwnsOne(b => b.Colour);

            base.Configure(builder);
        }
    }
}