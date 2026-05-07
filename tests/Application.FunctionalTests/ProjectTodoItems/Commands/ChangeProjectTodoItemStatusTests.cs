using CleanArchitecture.Application.Projects.Commands.CreateProject;
using CleanArchitecture.Application.ProjectTodoItems.Commands.ChangeProjectTodoItemStatus;
using CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.ProjectTodoItems.Commands;

public class ChangeProjectTodoItemStatusTests : TestBase
{
    [Test]
    public async Task ShouldChangeStatusAndQueueReporterEmail()
    {
        await TestApp.RunAsDefaultUserAsync();
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        var statuses = await TestApp.SendAsync(new CleanArchitecture.Application.ProjectTodoStatuses.Queries.GetProjectTodoStatuses.GetProjectTodoStatusesQuery(projectId));
        var targetStatus = statuses.First(s => s.Name == "In Progress");

        await TestApp.SendAsync(new ChangeProjectTodoItemStatusCommand
        {
            ProjectId = projectId,
            Id = itemId,
            StatusId = targetStatus.Id
        });

        var item = await TestApp.FindAsync<ProjectTodoItem>(itemId);

        item.ShouldNotBeNull();
        item!.StatusId.ShouldBe(targetStatus.Id);
        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(1);
    }

    [Test]
    public async Task ShouldNotQueueReporterEmailWhenStatusIsUnchanged()
    {
        await TestApp.RunAsDefaultUserAsync();
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        var itemId = await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        var statuses = await TestApp.SendAsync(new CleanArchitecture.Application.ProjectTodoStatuses.Queries.GetProjectTodoStatuses.GetProjectTodoStatusesQuery(projectId));
        var defaultStatus = statuses.First(s => s.IsDefault);

        await TestApp.SendAsync(new ChangeProjectTodoItemStatusCommand
        {
            ProjectId = projectId,
            Id = itemId,
            StatusId = defaultStatus.Id
        });

        (await TestApp.CountAsync<EmailOutboxMessage>()).ShouldBe(0);
    }
}
