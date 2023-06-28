using CleanArchitecture.Application.Common.Interfaces;
using MediatR;

namespace CleanArchitecture.Application.FeatureName.Queries.CleanArchitectureUseCase;

public record CleanArchitectureUseCaseQuery : IRequest<object>
{
}

public class CleanArchitectureUseCaseQueryHandler : IRequestHandler<CleanArchitectureUseCaseQuery, object>
{
    private readonly IApplicationDbContext _context;

    public CleanArchitectureUseCaseQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(CleanArchitectureUseCaseQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
