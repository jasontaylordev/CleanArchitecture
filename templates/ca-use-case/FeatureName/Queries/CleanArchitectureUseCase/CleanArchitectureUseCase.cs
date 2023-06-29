using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.FeatureName.Queries.CleanArchitectureUseCase;

public record CleanArchitectureUseCaseQuery : IRequest<object>
{
}

public class CleanArchitectureCommandCommandValidator : AbstractValidator<CleanArchitectureCommandCommand>
{
    public CleanArchitectureCommandCommandValidator()
    {
    }
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
