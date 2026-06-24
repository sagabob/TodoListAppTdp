using MediatR;
using TodoListApi.Models;

namespace TodoListApi.Application.Requests
{
    public record GetTodoListRequest(string Email): IRequest<Result<TodoList>>;

}
