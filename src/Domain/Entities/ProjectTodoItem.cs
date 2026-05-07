namespace CleanArchitecture.Domain.Entities;

public class ProjectTodoItem : BaseAuditableEntity
{
    public int ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly? DueDate { get; set; }

    public string? AssigneeUserId { get; set; }

    public string ReporterUserId { get; set; } = string.Empty;

    public int StatusId { get; set; }

    public Project Project { get; set; } = null!;

    public ProjectTodoStatus Status { get; set; } = null!;

    public bool AssignTo(string? assigneeUserId)
    {
        if (AssigneeUserId == assigneeUserId)
        {
            return false;
        }

        AssigneeUserId = assigneeUserId;

        if (!string.IsNullOrWhiteSpace(assigneeUserId))
        {
            AddDomainEvent(new ProjectTodoItemAssignedEvent(this));
        }

        return true;
    }

    public bool ChangeStatus(int statusId)
    {
        if (StatusId == statusId)
        {
            return false;
        }

        StatusId = statusId;
        AddDomainEvent(new ProjectTodoItemStatusChangedEvent(this));

        return true;
    }
}
