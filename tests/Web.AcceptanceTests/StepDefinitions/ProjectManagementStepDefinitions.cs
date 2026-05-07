namespace CleanArchitecture.Web.AcceptanceTests.StepDefinitions;

[Binding]
public sealed class ProjectManagementStepDefinitions(ProjectsPage projectsPage)
{
    private string _projectName = string.Empty;
    private string _projectDescription = string.Empty;
    private string _itemTitle = string.Empty;
    private string _itemDescription = string.Empty;
    private string _editedItemDescription = string.Empty;

    [BeforeFeature("ProjectManagement")]
    public static async Task BeforeProjectManagementFeature(IObjectContainer container)
    {
        var context = await PlaywrightSetup.Browser.NewContextAsync();
        var page = await context.NewPageAsync();

        var loginPage = new LoginPage(page);
        await loginPage.GotoAsync();
        await loginPage.SetEmail("administrator@localhost");
        await loginPage.SetPassword("Administrator1!");
        await loginPage.ClickLogin();
        await Assertions.Expect(page.Locator("a:has-text('Log out')")).ToBeVisibleAsync();

        container.RegisterInstanceAs(context);
        container.RegisterInstanceAs(new ProjectsPage(page));
    }

    [AfterFeature("ProjectManagement")]
    public static async Task AfterProjectManagementFeature(IObjectContainer container)
    {
        var context = container.Resolve<IBrowserContext>();
        await context.DisposeAsync();
    }

    [Given("an authenticated user visits the projects page")]
    public Task GivenAnAuthenticatedUserVisitsTheProjectsPage() => projectsPage.GotoAsync();

    [Given("an authenticated user opens a new project for project management")]
    public async Task GivenAnAuthenticatedUserOpensANewProjectForProjectManagement()
    {
        await projectsPage.GotoAsync();
        GenerateProjectData();
        await projectsPage.CreateProjectAsync(_projectName, _projectDescription);
        await projectsPage.OpenProjectAsync(_projectName);
    }

    [Then("the projects heading is {string}")]
    public Task ThenTheProjectsHeadingIs(string heading) => projectsPage.AssertProjectsHeading(heading);

    [Then("the seeded demo project is displayed")]
    public Task ThenTheSeededDemoProjectIsDisplayed() => projectsPage.AssertSeededDemoProjectVisible();

    [When("the user creates a unique project")]
    public async Task WhenTheUserCreatesAUniqueProject()
    {
        GenerateProjectData();
        await projectsPage.CreateProjectAsync(_projectName, _projectDescription);
    }

    [Then("the new project is displayed in the projects list")]
    public Task ThenTheNewProjectIsDisplayedInTheProjectsList() => projectsPage.AssertProjectVisibleInListAsync(_projectName);

    [When("the user opens the new project")]
    public Task WhenTheUserOpensTheNewProject() => projectsPage.OpenProjectAsync(_projectName);

    [Then("the project detail page is displayed")]
    public Task ThenTheProjectDetailPageIsDisplayed() => projectsPage.AssertProjectDetailVisibleAsync(_projectName);

    [When("the user creates a project to-do item")]
    public async Task WhenTheUserCreatesAProjectToDoItem()
    {
        GenerateItemData();
        await projectsPage.CreateItemAsync(_itemTitle, _itemDescription, DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)).ToString("yyyy-MM-dd"));
    }

    [Then("the item is displayed in the list view")]
    public Task ThenTheItemIsDisplayedInTheListView() => projectsPage.AssertItemVisibleInListAsync(_itemTitle);

    [When("the user edits the project to-do item description")]
    public async Task WhenTheUserEditsTheProjectToDoItemDescription()
    {
        _editedItemDescription = $"Updated acceptance description {UniqueSuffix()}";
        await projectsPage.EditItemDescriptionAsync(_itemTitle, _editedItemDescription);
    }

    [Then("the edited project to-do item description is displayed")]
    public Task ThenTheEditedProjectToDoItemDescriptionIsDisplayed() => projectsPage.AssertItemDescriptionVisibleAsync(_itemTitle, _editedItemDescription);

    [When("the user assigns the item to the current user")]
    public Task WhenTheUserAssignsTheItemToTheCurrentUser() => projectsPage.AssignItemToCurrentUserAsync(_itemTitle);

    [Then("the assignment remains selected for the item")]
    public Task ThenTheAssignmentRemainsSelectedForTheItem() => projectsPage.AssertItemAssignedToCurrentUserAsync(_itemTitle);

    [When("the user changes the item status to {string}")]
    public Task WhenTheUserChangesTheItemStatusTo(string statusName) => projectsPage.ChangeItemStatusAsync(_itemTitle, statusName);

    [When("the user opens the Kanban view")]
    public Task WhenTheUserOpensTheKanbanView() => projectsPage.OpenKanbanViewAsync();

    [Then("the item is displayed in the {string} Kanban column")]
    public Task ThenTheItemIsDisplayedInTheKanbanColumn(string statusName) => projectsPage.AssertItemVisibleInKanbanColumnAsync(_itemTitle, statusName);

    [When("the user opens the notifications page")]
    public Task WhenTheUserOpensTheNotificationsPage() => projectsPage.OpenNotificationsAsync();

    [Then("an assignment notification for the item is displayed")]
    public Task ThenAnAssignmentNotificationForTheItemIsDisplayed() => projectsPage.AssertAssignmentNotificationVisibleAsync(_itemTitle);

    [Then("a status change email for the item is displayed")]
    public Task ThenAStatusChangeEmailForTheItemIsDisplayed() => projectsPage.AssertStatusChangeEmailVisibleAsync(_itemTitle, "In Progress");

    private void GenerateProjectData()
    {
        var suffix = UniqueSuffix();
        _projectName = $"Acceptance Project {suffix}";
        _projectDescription = $"Project created by Web.AcceptanceTests {suffix}";
    }

    private void GenerateItemData()
    {
        var suffix = UniqueSuffix();
        _itemTitle = $"Acceptance item {suffix}";
        _itemDescription = $"Initial acceptance description {suffix}";
    }

    private static string UniqueSuffix() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
}
