using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodosByListId
{
    public class GetTodosByListIdQuery : PaginationQuery, IRequest<List<TodoItemResultRecord>>
    {
        public int ListId { get; set; }
    }

    public class GetTodosByListIdQueryHandler : IRequestHandler<GetTodosByListIdQuery, List<TodoItemResultRecord>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetTodosByListIdQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        async Task<List<TodoItemResultRecord>> IRequestHandler<GetTodosByListIdQuery, List<TodoItemResultRecord>>.Handle(GetTodosByListIdQuery request, CancellationToken cancellationToken)
        {
            if(request.SkipPageCount >= 0)
            {
                return await _context.TodoItems.Skip(request.SkipPageCount).Take(request.PageSize.GetValueOrDefault())
                    .Where(t => t.ListId == request.ListId)
                    .ProjectTo<TodoItemResultRecord>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }

            var records = await _context.TodoItems
                    .Where(t => t.ListId == request.ListId)
                    .ProjectTo<TodoItemResultRecord>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

            return records;
        }
    }
}
