using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace CleanArchitecture.Domain.UnitTests.ProjectManagement;

public class ProjectTodoItemTests
{
    [Test]
    public void AssignToShouldSetAssigneeAndRaiseEvent()
    {
        var item = new ProjectTodoItem { Title = "Task", ReporterUserId = "reporter", StatusId = 1 };

        var changed = item.AssignTo("assignee");

        changed.ShouldBeTrue();
        item.AssigneeUserId.ShouldBe("assignee");
        item.DomainEvents.OfType<ProjectTodoItemAssignedEvent>().ShouldHaveSingleItem();
    }

    [Test]
    public void AssignToShouldNotRaiseEventWhenAssigneeIsUnchanged()
    {
        var item = new ProjectTodoItem { Title = "Task", AssigneeUserId = "assignee", ReporterUserId = "reporter", StatusId = 1 };

        var changed = item.AssignTo("assignee");

        changed.ShouldBeFalse();
        item.AssigneeUserId.ShouldBe("assignee");
        item.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void AssignToShouldNotRaiseAssignmentEventWhenUnassigned()
    {
        var item = new ProjectTodoItem { Title = "Task", AssigneeUserId = "assignee", ReporterUserId = "reporter", StatusId = 1 };

        var changed = item.AssignTo(null);

        changed.ShouldBeTrue();
        item.AssigneeUserId.ShouldBeNull();
        item.DomainEvents.ShouldBeEmpty();
    }

    [Test]
    public void ChangeStatusShouldSetStatusAndRaiseEvent()
    {
        var item = new ProjectTodoItem { Title = "Task", ReporterUserId = "reporter", StatusId = 1 };

        var changed = item.ChangeStatus(2);

        changed.ShouldBeTrue();
        item.StatusId.ShouldBe(2);
        item.DomainEvents.OfType<ProjectTodoItemStatusChangedEvent>().ShouldHaveSingleItem();
    }

    [Test]
    public void ChangeStatusShouldNotRaiseEventWhenStatusIsUnchanged()
    {
        var item = new ProjectTodoItem { Title = "Task", ReporterUserId = "reporter", StatusId = 1 };

        var changed = item.ChangeStatus(1);

        changed.ShouldBeFalse();
        item.StatusId.ShouldBe(1);
        item.DomainEvents.ShouldBeEmpty();
    }
}
