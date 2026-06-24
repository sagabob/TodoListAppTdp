using Microsoft.EntityFrameworkCore;
using TodoListApi;
using TodoListApi.Mapping;
using TodoListApi.Models;
using TodoListApi.Repos;

namespace TodoListApi.Tests.Repos;

public class TodoListRepositoryTests : IDisposable
{
    private readonly TodoDbContext _dbContext;
    private readonly TodoListRepository _sut;

    public TodoListRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TodoDbContext(options);
        _sut = new TodoListRepository(_dbContext, TestConfiguration.CreateLogger<TodoListRepository>());
    }

    [Fact]
    public async Task UpsertAsync_CreatesTodoForUser()
    {
        var dto = new TodoItemDto
        {
            Title = "Buy milk",
            Description = "Full scream"
        };
        TodoItemMapper.EnsureId(dto);

        var created = await _sut.UpsertAsync(dto, "demo@test.com", CancellationToken.None);

        Assert.True(created);
        Assert.NotNull(dto.Id);

        var stored = await _dbContext.TodoItems.SingleAsync();
        Assert.Equal(dto.Id, stored.Id);
        Assert.Equal("demo@test.com", stored.UserName);
        Assert.Equal("Buy milk", stored.Title);
        Assert.Equal("Full scream", stored.Description);
        Assert.False(stored.IsCompleted);
    }

    [Fact]
    public async Task GetTodoListAsync_ReturnsOnlyUserTodos()
    {
        _dbContext.TodoItems.AddRange(
            new TodoItem { Id = Guid.NewGuid(), Title = "Mine", UserName = "demo@test.com" },
            new TodoItem { Id = Guid.NewGuid(), Title = "Theirs", UserName = "other@test.com" });
        await _dbContext.SaveChangesAsync();

        var list = await _sut.GetTodoListAsync("demo@test.com", CancellationToken.None);

        Assert.Single(list.Items);
        Assert.Equal("Mine", list.Items[0].Title);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingTodo()
    {
        var id = Guid.NewGuid();
        _dbContext.TodoItems.Add(new TodoItem
        {
            Id = id,
            Title = "Delete me",
            UserName = "demo@test.com"
        });
        await _dbContext.SaveChangesAsync();

        var deleted = await _sut.DeleteAsync(id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Empty(await _dbContext.TodoItems.ToListAsync());
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenNotFound()
    {
        var deleted = await _sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(deleted);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
