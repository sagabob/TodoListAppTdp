using TodoListApi.Models;

namespace TodoListApi.Mapping;

public static class TodoItemMapper
{
    public static TodoItemDto ToDto(TodoItem entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        IsCompleted = entity.IsCompleted
    };

    public static TodoItem ToEntity(TodoItemDto dto, string userName) => new()
    {
        Id = ResolveId(dto),
        Title = dto.Title,
        Description = dto.Description,
        IsCompleted = dto.IsCompleted,
        UserName = userName,
        CreatedAt = DateTime.UtcNow,
        LastUpdate = DateTime.UtcNow
    };

    public static void ApplyUpdate(TodoItemDto dto, TodoItem existing)
    {
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.IsCompleted = dto.IsCompleted;
        existing.LastUpdate = DateTime.UtcNow;
    }

    public static void EnsureId(TodoItemDto dto)
    {
        if (dto.Id is null || dto.Id == Guid.Empty)
            dto.Id = Guid.NewGuid();
    }

    private static Guid ResolveId(TodoItemDto dto) =>
        dto.Id is null || dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id.Value;
}
