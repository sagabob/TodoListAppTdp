namespace TodoListApi.Models
{
    public class UpsertTodoItemDtoResult
    {
        public required TodoItemDto Item { get; init; }
        public bool WasCreated { get; init; }
    }
}
