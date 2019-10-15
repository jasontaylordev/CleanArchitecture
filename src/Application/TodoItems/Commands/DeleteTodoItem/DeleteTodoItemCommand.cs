using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem
{
    public class DeleteTodoItemCommand : IRequest
    {
        public long Id { get; set; }

        public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
        {
            private readonly IApplicationDbContext _context;

            public DeleteTodoItemCommandHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.TodoItems.FindAsync(request.Id);

                // TODO: Check for Null

                _context.TodoItems.Remove(entity);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
