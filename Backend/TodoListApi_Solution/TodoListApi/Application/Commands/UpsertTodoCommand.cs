using MediatR;
using TodoListApi.Models;

namespace TodoListApi.Application.Commands
{
    public record UpsertTodoCommand(TodoItemDto Item, string UserName) : IRequest<Result<UpsertTodoItemDtoResult>>;
}
