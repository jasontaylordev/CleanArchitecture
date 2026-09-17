using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.FeatureName.Queries.CleanArchitectureUseCase;

//#if (hasReturnType)
public record CleanArchitectureUseCaseQuery : IRequest<TReturnType>
//#else
public record CleanArchitectureUseCaseQuery : IRequest
//#endif
{
}

public class CleanArchitectureUseCaseQueryValidator : AbstractValidator<CleanArchitectureUseCaseQuery>
{
    public CleanArchitectureUseCaseQueryValidator()
    {
    }
}

//#if (hasReturnType)
public class CleanArchitectureUseCaseQueryHandler : IRequestHandler<CleanArchitectureUseCaseQuery, TReturnType>
//#else
public class CleanArchitectureUseCaseQueryHandler : IRequestHandler<CleanArchitectureUseCaseQuery>
//#endif
{
    private readonly IApplicationDbContext _context;

    public CleanArchitectureUseCaseQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

//#if (hasReturnType)
    public async Task<TReturnType> Handle(CleanArchitectureUseCaseQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
//#else
    public async Task Handle(CleanArchitectureUseCaseQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
//#endif
}
