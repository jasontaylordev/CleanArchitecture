using CleanArchitecture.Application.Common.Interfaces;
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

                // TODO: Check for null

                entity.Name = request.Name;
                entity.IsComplete = request.IsComplete;

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
