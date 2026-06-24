using MediatR;
using TodoListApi.Models;
using TodoListApi.Repos;

namespace TodoListApi.Application.Requests;

public class GetTodoListRequestHandler(
    ITodoListRepository repository,
    ILogger<GetTodoListRequestHandler> logger)
    : IRequestHandler<GetTodoListRequest, Result<TodoList>>
{
    public async Task<Result<TodoList>> Handle(GetTodoListRequest request, CancellationToken cancellationToken)
    {
        var todoList = await repository.GetTodoListAsync(request.Email, cancellationToken);

        logger.LogInformation(
            "Retrieved {ItemCount} todo items for user {UserName}",
            todoList.Items.Count,
            request.Email);

        return Result<TodoList>.Success(todoList);
    }
}