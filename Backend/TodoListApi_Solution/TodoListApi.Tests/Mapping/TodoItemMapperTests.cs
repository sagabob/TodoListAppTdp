using TodoListApi.Mapping;
using TodoListApi.Models;

namespace TodoListApi.Tests.Mapping;

public class TodoItemMapperTests
{
    [Fact]
    public void ToDto_MapsEntityWithoutUserName()
    {
        var entity = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Desc",
            IsCompleted = true,
            UserName = "demo@test.com"
        };

        var dto = TodoItemMapper.ToDto(entity);

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Title, dto.Title);
        Assert.Equal(entity.Description, dto.Description);
        Assert.Equal(entity.IsCompleted, dto.IsCompleted);
    }

    [Fact]
    public void ToEntity_SetsUserNameFromParameter()
    {
        var dto = new TodoItemDto
        {
            Title = "New todo",
            Description = "Details"
        };

        var entity = TodoItemMapper.ToEntity(dto, "demo@test.com");

        Assert.Equal("demo@test.com", entity.UserName);
        Assert.Equal(dto.Title, entity.Title);
        Assert.NotEqual(Guid.Empty, entity.Id);
    }

    [Fact]
    public void EnsureId_GeneratesIdWhenMissing()
    {
        var dto = new TodoItemDto { Title = "Todo" };

        TodoItemMapper.EnsureId(dto);

        Assert.NotNull(dto.Id);
        Assert.NotEqual(Guid.Empty, dto.Id);
    }

    [Fact]
    public void ApplyUpdate_UpdatesMutableFields()
    {
        var existing = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = "Old",
            Description = "Old desc",
            IsCompleted = false,
            UserName = "demo@test.com"
        };
        var originalLastUpdate = existing.LastUpdate;

        var dto = new TodoItemDto
        {
            Id = existing.Id,
            Title = "Updated",
            Description = "Updated desc",
            IsCompleted = true
        };

        TodoItemMapper.ApplyUpdate(dto, existing);

        Assert.Equal("Updated", existing.Title);
        Assert.Equal("Updated desc", existing.Description);
        Assert.True(existing.IsCompleted);
        Assert.True(existing.LastUpdate >= originalLastUpdate);
    }
}
