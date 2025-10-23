using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteProductCommand : IRequest<bool>
{
    public string Name { get; set; } = default!;
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

        if (entity == null)
            return false;

        _context.Products.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
