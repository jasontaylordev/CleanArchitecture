using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Projects.Commands.CreateProject;
using CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.ProjectTodoItems.Commands;

public class CreateProjectTodoItemTests : TestBase
{
    [Test]
    public async Task ShouldRequireAuthenticatedReporter()
    {
        var command = new CreateProjectTodoItemCommand
        {
            ProjectId = 1,
            Title = "Task"
        };

        await Should.ThrowAsync<ForbiddenAccessException>(() => TestApp.SendAsync(command));
    }

    [Test]
    public async Task ShouldCreateItemWithCurrentUserAsReporter()
    {
        var userId = await TestApp.RunAsDefaultUserAsync();
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var dueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand
        {
            ProjectId = projectId,
            Title = "Prepare review",
            Description = "Review the implementation",
            DueDate = dueDate.ToString("yyyy-MM-dd")
        });

        var item = await TestApp.FindAsync<ProjectTodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.ProjectId.ShouldBe(projectId);
        item.Title.ShouldBe("Prepare review");
        item.DueDate.ShouldBe(dueDate);
        item.ReporterUserId.ShouldBe(userId);
        item.StatusId.ShouldBeGreaterThan(0);
    }
    [Test]
    public async Task ShouldQueueAssignmentEmailWhenCreatedWithAssignee()
    {
        await TestApp.RunAsDefaultUserAsync();
        var assigneeUserId = await TestApp.RunAsUserAsync("assignee.create@test.local", "Testing1234!", []);
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });

        await TestApp.SendAsync(new CreateProjectTodoItemCommand
        {
            ProjectId = projectId,
            Title = "Prepare review",
            AssigneeUserId = assigneeUserId
        });

        (await TestApp.CountAsync<UserNotification>()).ShouldBe(1);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(1);
    }
}
