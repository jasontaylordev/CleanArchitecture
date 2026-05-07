namespace CleanArchitecture.Web.AcceptanceTests.Pages;

public class ProjectsPage(IPage page) : BasePage(page)
{
    private const string CurrentUserName = "administrator@localhost";

    public override string PagePath => $"{BaseUrl}/projects";

    public async Task AssertProjectsHeading(string text)
    {
        await Assertions.Expect(Page.Locator("h1")).ToHaveTextAsync(text);
        await WaitForProjectsLoadAsync();
    }

    public async Task AssertSeededDemoProjectVisible()
    {
        await WaitForProjectsLoadAsync();
        await Assertions.Expect(ProjectCard("Demo Project")).ToBeVisibleAsync();
    }

    public async Task CreateProjectAsync(string name, string description)
    {
        await WaitForProjectsLoadAsync();
        await Page.GetByPlaceholder("Project name").FillAsync(name);
        await Page.GetByPlaceholder("Optional description").FillAsync(description);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create project" }).ClickAsync();
        await Assertions.Expect(ProjectDetailHeading(name)).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15_000 });
        await WaitForProjectItemsLoadAsync();
    }

    public async Task OpenProjectAsync(string name)
    {
        if (await ProjectDetailHeading(name).IsVisibleAsync())
        {
            await WaitForProjectItemsLoadAsync();
            return;
        }

        await GotoAsync();
        await WaitForProjectsLoadAsync();
        await ProjectCard(name).ClickAsync();
        await Assertions.Expect(ProjectDetailHeading(name)).ToBeVisibleAsync();
        await WaitForProjectItemsLoadAsync();
    }

    public async Task AssertProjectVisibleInListAsync(string name)
    {
        await GotoAsync();
        await WaitForProjectsLoadAsync();
        await Assertions.Expect(ProjectCard(name)).ToBeVisibleAsync();
    }

    public Task AssertProjectDetailVisibleAsync(string name)
        => Assertions.Expect(ProjectDetailHeading(name)).ToBeVisibleAsync();

    public async Task CreateItemAsync(string title, string description, string dueDate)
    {
        await WaitForProjectItemsLoadAsync();
        await Page.GetByPlaceholder("Task title").FillAsync(title);
        await Page.GetByPlaceholder("Describe the task, requirements, acceptance notes, or implementation details").FillAsync(description);
        await Page.Locator(".item-editor-card input[type='date']").FillAsync(dueDate);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Create item" }).ClickAsync();
        await AssertItemVisibleInListAsync(title);
    }

    public Task AssertItemVisibleInListAsync(string title)
        => Assertions.Expect(ItemRow(title)).ToBeVisibleAsync();

    public async Task EditItemDescriptionAsync(string title, string description)
    {
        await ItemRow(title).GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await Assertions.Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Edit item" })).ToBeVisibleAsync();
        await Page.Locator(".item-editor-card textarea").FillAsync(description);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save item" }).ClickAsync();
        await Assertions.Expect(ItemRow(title).GetByText(description)).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions { Timeout = 15_000 });
    }

    public Task AssertItemDescriptionVisibleAsync(string title, string description)
        => Assertions.Expect(ItemRow(title).GetByText(description)).ToBeVisibleAsync();

    public async Task AssignItemToCurrentUserAsync(string title)
    {
        var assigneeSelect = AssigneeSelect(title);
        await assigneeSelect.SelectOptionAsync(new[] { new SelectOptionValue { Label = CurrentUserName } });
        await Assertions.Expect(assigneeSelect.Locator("option:checked")).ToHaveTextAsync(CurrentUserName, new LocatorAssertionsToHaveTextOptions { Timeout = 15_000 });
    }

    public Task AssertItemAssignedToCurrentUserAsync(string title)
        => Assertions.Expect(AssigneeSelect(title).Locator("option:checked")).ToHaveTextAsync(CurrentUserName);

    public async Task ChangeItemStatusAsync(string title, string statusName)
    {
        var statusSelect = StatusSelect(title);
        await statusSelect.SelectOptionAsync(new[] { new SelectOptionValue { Label = statusName } });
        await Assertions.Expect(statusSelect.Locator("option:checked")).ToHaveTextAsync(statusName, new LocatorAssertionsToHaveTextOptions { Timeout = 15_000 });
    }

    public async Task OpenKanbanViewAsync()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kanban" }).ClickAsync();
        await Assertions.Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Kanban board" })).ToBeVisibleAsync();
    }

    public Task AssertItemVisibleInKanbanColumnAsync(string title, string statusName)
        => Assertions.Expect(Page.Locator($"xpath=//section[contains(@class,'kanban-column')][.//h3[normalize-space()={XPathLiteral(statusName)}]]//*[contains(@class,'kanban-card-title') and normalize-space()={XPathLiteral(title)}]")).ToBeVisibleAsync();

    public async Task OpenNotificationsAsync()
    {
        await Page.GotoAsync($"{BaseUrl}/notifications", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 60_000
        });
    }

    public async Task AssertAssignmentNotificationVisibleAsync(string title)
    {
        await Assertions.Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Notifications", Exact = true })).ToBeVisibleAsync();
        await Assertions.Expect(AssignmentNotificationsSection()).ToBeVisibleAsync();
        await Assertions.Expect(AssignmentNotificationsSection().Locator($"xpath=.//p[normalize-space()={XPathLiteral($"You have been assigned to '{title}'.")}]")).ToBeVisibleAsync();
    }

    public async Task AssertStatusChangeEmailVisibleAsync(string title, string statusName)
    {
        await Assertions.Expect(DevelopmentEmailOutboxSection()).ToBeVisibleAsync();
        await Assertions.Expect(DevelopmentEmailOutboxSection().GetByText($"The status for '{title}' changed to '{statusName}'.", new() { Exact = false })).ToBeVisibleAsync();
    }

    private ILocator ProjectCard(string name)
        => Page.Locator("article.project-card", new() { HasTextString = name });

    private ILocator AssignmentNotificationsSection()
        => Page.Locator("xpath=//article[./header/h2[normalize-space()='Assignment notifications']]");

    private ILocator DevelopmentEmailOutboxSection()
        => Page.Locator("xpath=//article[./header/h2[normalize-space()='Development email outbox']]");

    private ILocator ProjectDetailHeading(string name)
        => Page.GetByRole(AriaRole.Heading, new() { Name = name });

    private ILocator ItemRow(string title)
        => Page.Locator("tbody tr", new() { HasTextString = title });

    private ILocator AssigneeSelect(string title)
        => ItemRow(title).Locator("td.select-cell select").Nth(0);

    private ILocator StatusSelect(string title)
        => ItemRow(title).Locator("td.select-cell select").Nth(1);

    private Task WaitForProjectsLoadAsync()
        => Assertions.Expect(Page.GetByText("Loading projects...")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 15_000 });

    private Task WaitForProjectItemsLoadAsync()
        => Assertions.Expect(Page.GetByText("Loading project items...")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 15_000 });

    private static string XPathLiteral(string value)
    {
        if (!value.Contains('\''))
        {
            return $"'{value}'";
        }

        if (!value.Contains('"'))
        {
            return $"\"{value}\"";
        }

        return "concat('" + value.Replace("'", "', \"'\", '") + "')";
    }
}
