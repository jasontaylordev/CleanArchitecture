using CleanArchitecture.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.AggregateName.Commands.CleanArchitectureCommand;

public record CleanArchitectureCommandCommand : IRequest<Unit>
{
    
}

public class CleanArchitectureCommandCommandHandler : IRequestHandler<CleanArchitectureCommandCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public CleanArchitectureCommandCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CleanArchitectureCommandCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
