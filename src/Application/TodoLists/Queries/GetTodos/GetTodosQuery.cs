using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoLists.Queries.GetTodos
{
    public class GetTodosQuery : IRequest<TodosVm>
    {
        public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
            {
                var vm = new TodosVm();

                vm.Lists = await _context.TodoLists
                    .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                    .OrderBy(t => t.Title)
                    .ToListAsync(cancellationToken);

                return vm;
            }
        }
    }
}
