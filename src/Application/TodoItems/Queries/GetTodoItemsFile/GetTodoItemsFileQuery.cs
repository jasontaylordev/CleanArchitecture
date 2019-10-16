using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile
{
    public class GetTodoItemsFileQuery : IRequest<TodoItemsFileVm>
    {
        public class GetTodoItemsFileQueryHandler : IRequestHandler<GetTodoItemsFileQuery, TodoItemsFileVm>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ICsvFileBuilder _fileBuilder;

            public GetTodoItemsFileQueryHandler(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
            {
                _context = context;
                _mapper = mapper;
                _fileBuilder = fileBuilder;
            }

            public async Task<TodoItemsFileVm> Handle(GetTodoItemsFileQuery request, CancellationToken cancellationToken)
            {
                var records = await _context.TodoItems
                   .ProjectTo<TodoItemFileRecord>(_mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken);

                var fileContent = _fileBuilder.BuildTodoItemsFile(records);

                var vm = new TodoItemsFileVm
                {
                    Content = fileContent,
                    ContentType = "text/csv",
                    FileName = "TodoItems.csv"
                };

                return vm;
            }
        }
    }
}
