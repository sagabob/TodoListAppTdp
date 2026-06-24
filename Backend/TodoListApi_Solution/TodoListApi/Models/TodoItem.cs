namespace TodoListApi.Models;

public class TodoItem
{
    public Guid Id { get; set; }

    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; } = false;

    public required string UserName { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
}