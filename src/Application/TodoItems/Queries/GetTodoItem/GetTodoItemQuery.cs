using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItem
{
    public class GetTodoItemQuery : IRequest<TodoItemVm>
    {
        public long Id { get; set; }

        public class GetTodoItemQueryHandler : IRequestHandler<GetTodoItemQuery, TodoItemVm>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetTodoItemQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<TodoItemVm> Handle(GetTodoItemQuery request, CancellationToken cancellationToken)
            {
                var vm = await _context.TodoItems
                    .Where(t => t.Id == request.Id)
                    .ProjectTo<TodoItemVm>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (vm == null)
                {
                    throw new NotFoundException(nameof(TodoItem), request.Id);
                }

                return vm;
            }
        }
    }
}
