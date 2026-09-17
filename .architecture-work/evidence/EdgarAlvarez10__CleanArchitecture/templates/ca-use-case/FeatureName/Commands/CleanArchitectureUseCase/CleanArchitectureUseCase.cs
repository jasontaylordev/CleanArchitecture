using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.FeatureName.Commands.CleanArchitectureUseCase;

//#if (hasReturnType)
public record CleanArchitectureUseCaseCommand : IRequest<TReturnType>
//#else
public record CleanArchitectureUseCaseCommand : IRequest
//#endif
{
}

public class CleanArchitectureUseCaseCommandValidator : AbstractValidator<CleanArchitectureUseCaseCommand>
{
    public CleanArchitectureUseCaseCommandValidator()
    {
    }
}

//#if (hasReturnType)
public class CleanArchitectureUseCaseCommandHandler : IRequestHandler<CleanArchitectureUseCaseCommand, TReturnType>
//#else
public class CleanArchitectureUseCaseCommandHandler : IRequestHandler<CleanArchitectureUseCaseCommand>
//#endif
{
    private readonly IApplicationDbContext _context;

    public CleanArchitectureUseCaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

//#if (hasReturnType)
    public async Task<TReturnType> Handle(CleanArchitectureUseCaseCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
//#else
    public async Task Handle(CleanArchitectureUseCaseCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
//#endif
}
