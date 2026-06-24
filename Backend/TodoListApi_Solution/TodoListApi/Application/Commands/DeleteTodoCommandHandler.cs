using MediatR;
using TodoListApi.Repos;

namespace TodoListApi.Application.Commands;

public class DeleteTodoCommandHandler(
    ITodoListRepository repository,
    ILogger<DeleteTodoCommandHandler> logger) : IRequestHandler<DeleteTodoCommand, Result>
{
    public async Task<Result> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(request.Id, cancellationToken);

        if (!deleted)
        {
            logger.LogWarning("Todo item {TodoId} not found for delete", request.Id);
            return Result.Failure("Todo item not found");
        }

        logger.LogInformation("Deleted todo item {TodoId}", request.Id);
        return Result.Success();
    }
}
