using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Mappings;

public static class MappingExtensions
{
    public static Task<PaginatedList<TDestination>> PaginatedListAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Func<TSource, TDestination> mapFunc,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where TSource : class
        where TDestination : class
    {
        return PaginatedList<TDestination>.CreateAsync(queryable.AsNoTracking(), mapFunc, pageNumber, pageSize, cancellationToken);
    }

    public static async Task<List<TDestination>> ProjectToListAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Func<TSource, TDestination> mapFunc,
        CancellationToken cancellationToken = default)
        where TSource : class
        where TDestination : class
    {
        return await queryable
            .AsNoTracking()
            .Select(x => mapFunc(x))
            .ToListAsync(cancellationToken);
    }
}
