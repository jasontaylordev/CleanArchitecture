using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem
{
    public class CreateTodoItemCommand : IRequest<long>
    {
        public string Name { get; set; }

        public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, long>
        {
            private readonly IApplicationDbContext _context;

            public CreateTodoItemCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<long> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
            {
                var entity = new TodoItem
                {
                    Name = request.Name,
                    IsComplete = false
                };

                _context.TodoItems.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return entity.Id;
            }
        }
    }
}
