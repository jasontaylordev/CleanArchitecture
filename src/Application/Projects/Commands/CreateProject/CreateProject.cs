using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(v => v.Description)
            .MaximumLength(1000);
    }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description
        };

        project.Statuses.Add(new ProjectTodoStatus { Name = "To Do", SortOrder = 10, IsDefault = true });
        project.Statuses.Add(new ProjectTodoStatus { Name = "In Progress", SortOrder = 20 });
        project.Statuses.Add(new ProjectTodoStatus { Name = "Done", SortOrder = 30, IsTerminal = true });

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(cancellationToken);

        return project.Id;
    }
}
