using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Test]
    public async Task AddEntity_ShouldNotPersistWithoutSaveChanges()
    {
        // Arrange
        var todoName = "Test Name";
        var todoColor = Colour.Blue;

        var todoList = new TodoList()
        {
            Title = todoName,
            Colour = todoColor,
        };

        // Act
        await dbContext.AddAsync(todoList);

        // Assert before SaveChanges
        var entityEntry = dbContext.Entry(todoList);
        entityEntry.State.Should().Be(EntityState.Added);

        // Optionally, you can also check if it is in the context
        var newTodo = await dbContext.TodoLists.FirstOrDefaultAsync(t => t.Title == todoName);
        newTodo.Should().BeNull("because the changes have not been saved to the database yet.");
    }

    [Test]
    public async Task AddEntity_ShouldPersistAfterSaveChanges()
    {
        // Arrange
        var todoName = "Test Name";
        var todoColor = Colour.Blue;

        var todoList = new TodoList()
        {
            Title = todoName,
            Colour = todoColor,
        };

        // Act
        await dbContext.AddAsync(todoList);
        await dbContext.SaveChangesAsync();

        // Assert
        var newTodo = await dbContext.TodoLists.FirstOrDefaultAsync();

        newTodo.Should().NotBeNull();
        newTodo?.Title.Should().NotBeNull();
        newTodo?.Title.Should().Be(todoName);
        newTodo?.Colour.Code.Should().Be(todoColor.Code);
    }

}
