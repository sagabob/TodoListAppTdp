using TodoListApi.Models;

namespace TodoListApi.Repos
{
    public interface ITodoListRepository
    {
        Task<TodoItemDto?> GetTodoItemByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<TodoList> GetTodoListAsync(string email, CancellationToken cancellationToken);

        Task<bool> UpsertAsync(TodoItemDto item, string userName, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);

    }
}
