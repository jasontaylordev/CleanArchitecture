using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpdateProductCommand : IRequest<bool>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Products
            .FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);

        if (entity == null)
            return false;

        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.ImageUrl = request.ImageUrl;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
