using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList
{
    public class GetTodoItemsListQuery : IRequest<TodoItemsListVm>
    {
        public class GetTodoItemsListQueryHandler : IRequestHandler<GetTodoItemsListQuery, TodoItemsListVm>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetTodoItemsListQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<TodoItemsListVm> Handle(GetTodoItemsListQuery request, CancellationToken cancellationToken)
            {
                var items = await _context.TodoItems
                    .ProjectTo<TodoItemDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                var vm = new TodoItemsListVm
                {
                    TodoItems = items
                };

                return vm;
            }
        }
    }
}
