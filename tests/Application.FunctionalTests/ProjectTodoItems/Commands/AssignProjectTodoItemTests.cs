using CleanArchitecture.Application.Projects.Commands.CreateProject;
using CleanArchitecture.Application.ProjectTodoItems.Commands.AssignProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.ProjectTodoItems.Commands;

public class AssignProjectTodoItemTests : TestBase
{
    [Test]
    public async Task ShouldAssignItemAndCreateNotification()
    {
        await TestApp.RunAsDefaultUserAsync();
        var assigneeUserId = await TestApp.RunAsUserAsync("assignee@test.local", "Testing1234!", []);
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = assigneeUserId
        });

        var item = await TestApp.FindAsync<ProjectTodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.AssigneeUserId.ShouldBe(assigneeUserId);
        (await TestApp.CountAsync<UserNotification>()).ShouldBe(1);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldNotCreateDuplicateNotificationWhenAssigneeIsUnchanged()
    {
        await TestApp.RunAsDefaultUserAsync();
        var assigneeUserId = await TestApp.RunAsUserAsync("assignee@test.local", "Testing1234!", []);
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = assigneeUserId
        });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = assigneeUserId
        });

        (await TestApp.CountAsync<UserNotification>()).ShouldBe(1);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldQueueAssignmentAndUnassignmentEmailsWhenAssigneeChanges()
    {
        await TestApp.RunAsDefaultUserAsync();
        var firstAssigneeUserId = await TestApp.RunAsUserAsync("first.assignee@test.local", "Testing1234!", []);
        var secondAssigneeUserId = await TestApp.RunAsUserAsync("second.assignee@test.local", "Testing1234!", []);
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = firstAssigneeUserId
        });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = secondAssigneeUserId
        });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = null
        });

        var item = await TestApp.FindAsync<ProjectTodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.AssigneeUserId.ShouldBeNull();
        (await TestApp.CountAsync<UserNotification>()).ShouldBe(2);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(4);
    }

    [Test]
    public async Task ShouldNotCreateNotificationWhenUnassigned()
    {
        await TestApp.RunAsDefaultUserAsync();
        var assigneeUserId = await TestApp.RunAsUserAsync("assignee@test.local", "Testing1234!", []);
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand
        {
            ProjectId = projectId,
            Title = "Task",
            AssigneeUserId = assigneeUserId
        });

        await TestApp.SendAsync(new AssignProjectTodoItemCommand
        {
            ProjectId = projectId,
            Id = itemId,
            AssigneeUserId = null
        });

        var item = await TestApp.FindAsync<ProjectTodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.AssigneeUserId.ShouldBeNull();
        (await TestApp.CountAsync<UserNotification>()).ShouldBe(1);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(2);
    }
}
