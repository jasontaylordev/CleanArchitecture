using CleanArchitecture.Application.Projects.Commands.CreateProject;
using CleanArchitecture.Application.ProjectTodoItems.Commands.CreateProjectTodoItem;
using CleanArchitecture.Application.ProjectTodoItems.Queries.GetProjectKanbanBoard;

namespace CleanArchitecture.Application.FunctionalTests.ProjectTodoItems.Queries;

public class GetProjectKanbanBoardTests : TestBase
{
    [Test]
    public async Task ShouldGroupItemsByExtensibleStatuses()
    {
        await TestApp.RunAsDefaultUserAsync();
        var projectId = await TestApp.SendAsync(new CreateProjectCommand { Name = "Project" });
        await TestApp.SendAsync(new CreateProjectTodoItemCommand { ProjectId = projectId, Title = "Task" });

        var board = await TestApp.SendAsync(new GetProjectKanbanBoardQuery(projectId));

        board.ProjectId.ShouldBe(projectId);
        board.Columns.Select(c => c.StatusName).ShouldBe(new[] { "To Do", "In Progress", "Done" });
        board.Columns.Sum(c => c.Items.Count).ShouldBe(1);
    }
}
