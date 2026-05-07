namespace CleanArchitecture.Domain.Entities;

public class Project : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public IList<ProjectTodoStatus> Statuses { get; private set; } = new List<ProjectTodoStatus>();

    public IList<ProjectTodoItem> TodoItems { get; private set; } = new List<ProjectTodoItem>();
}
