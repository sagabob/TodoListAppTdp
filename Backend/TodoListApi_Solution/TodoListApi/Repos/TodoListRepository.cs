using Microsoft.EntityFrameworkCore;
using TodoListApi.Mapping;
using TodoListApi.Models;

namespace TodoListApi.Repos;

public class TodoListRepository(
    TodoDbContext dbContext,
    ILogger<TodoListRepository> logger) : ITodoListRepository
{
    public async Task<TodoItemDto?> GetTodoItemByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Querying todo item by id {TodoId}", id);

        var selectedItem = await dbContext.TodoItems.AsNoTracking().Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return selectedItem is null ? null : TodoItemMapper.ToDto(selectedItem);
    }

    public async Task<TodoList> GetTodoListAsync(string email, CancellationToken cancellationToken)
    {
        logger.LogDebug("Querying todo list for user {UserName}", email);

        var todoList = await dbContext.TodoItems.AsNoTracking().Where(x => x.UserName == email)
            .Select(x => new TodoItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                IsCompleted = x.IsCompleted
            })
            .ToListAsync(cancellationToken);

        return new TodoList(todoList);
    }

    public async Task<bool> UpsertAsync(TodoItemDto item, string userName, CancellationToken cancellationToken)
    {
        logger.LogDebug("Upserting todo item {TodoId}", item.Id);

        var existing = await dbContext.TodoItems.Where(x => x.Id == item.Id).FirstOrDefaultAsync(cancellationToken);
        var isCreated = existing is null;

        if (existing is null)
        {
            var toDoItem = TodoItemMapper.ToEntity(item, userName);
            dbContext.TodoItems.Add(toDoItem);
        }
        else
        {
            TodoItemMapper.ApplyUpdate(item, existing);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return isCreated;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        logger.LogDebug("Deleting todo item {TodoId}", id);

        var existing = await dbContext.TodoItems.Where(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
            return false;

        dbContext.TodoItems.Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
