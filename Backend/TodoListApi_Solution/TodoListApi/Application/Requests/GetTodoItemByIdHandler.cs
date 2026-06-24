using MediatR;
using TodoListApi.Models;
using TodoListApi.Repos;

namespace TodoListApi.Application.Requests
{
    public class GetTodoItemByIdHandler(
        ITodoListRepository repos,
        ILogger<GetTodoItemByIdHandler> logger) : IRequestHandler<GetTodoItemByIdRequest, Result<TodoItemDto>>
    {
        public async Task<Result<TodoItemDto>> Handle(GetTodoItemByIdRequest request, CancellationToken cancellationToken)
        {
            var item = await repos.GetTodoItemByIdAsync(request.Id, cancellationToken);

            if (item is null)
            {
                logger.LogWarning("Todo item {TodoId} not found", request.Id);
                return Result<TodoItemDto>.Failure("Todo item not found");
            }

            return Result<TodoItemDto>.Success(item);
        }
    }
}
