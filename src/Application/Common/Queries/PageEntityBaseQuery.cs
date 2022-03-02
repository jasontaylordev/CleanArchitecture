using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Common.Queries
{
    public class PageEntityBaseQuery<TEntityDto> : IRequest<PaginatedList<TEntityDto>>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }

    public abstract class PageEntityBaseQueryHandler<Request, TEntity, TEntityDto> : IRequestHandler<Request, PaginatedList<TEntityDto>>
    where Request : PageEntityBaseQuery<TEntityDto>
    where TEntity : class
    {
        protected readonly IApplicationDbContext Context;

        protected readonly IMapper Mapper;

        protected DbSet<TEntity> Entity => Context.Set<TEntity>();

        protected abstract IQueryable<TEntity> Query(Request request);

        public PageEntityBaseQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<PaginatedList<TEntityDto>> Handle(Request request, CancellationToken cancellationToken)
        {

            return await Query(request)
            .ProjectTo<TEntityDto>(Mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}