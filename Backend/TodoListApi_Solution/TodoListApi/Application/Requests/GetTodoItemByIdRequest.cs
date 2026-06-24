using MediatR;
using TodoListApi.Models;

namespace TodoListApi.Application.Requests
{
    public record GetTodoItemByIdRequest(Guid Id) : IRequest<Result<TodoItemDto>>;

}
