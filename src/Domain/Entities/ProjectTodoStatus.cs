namespace CleanArchitecture.Domain.Entities;

public class ProjectTodoStatus : BaseAuditableEntity
{
    public int ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public bool IsDefault { get; set; }

    public bool IsTerminal { get; set; }

    public Project Project { get; set; } = null!;

    public IList<ProjectTodoItem> TodoItems { get; private set; } = new List<ProjectTodoItem>();
}
