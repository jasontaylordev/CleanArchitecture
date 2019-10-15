using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    public class UpdateTodoItemCommand : IRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool IsComplete { get; set; }

        public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
        {
            private readonly IApplicationDbContext _context;

            public UpdateTodoItemCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.TodoItems.FindAsync(request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(TodoItem), request.Id);
                }

                entity.Name = request.Name;
                entity.IsComplete = request.IsComplete;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
