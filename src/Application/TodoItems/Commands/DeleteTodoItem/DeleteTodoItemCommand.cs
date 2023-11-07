using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        _context.TodoItems.Remove(entity);

        entity.AddDomainEvent(new BaseEvent(entity)
        {
            Level = LogLevel.Warning,
            Message = "Deleted To Do Item"
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

}
