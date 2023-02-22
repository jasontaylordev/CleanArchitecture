
using CleanArchitecture.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CleanArchitecture.Infrastructure.Persistence
{
    public class DbSchemaAwareModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context, bool designTime)
        {
            return new
            {
                Type = context.GetType(),
                Schema = context is IDbContextSchema schema ? schema.Schema : null
            };
        }
    }
}