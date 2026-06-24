namespace TodoListApi.Models;

public class TodoList(List<TodoItemDto> todoList)
{
    public IReadOnlyList<TodoItemDto> Items { get; init; } = todoList;
}