using MediatR;
using TodoListApi.Mapping;
using TodoListApi.Models;
using TodoListApi.Repos;

namespace TodoListApi.Application.Commands
{
    public class UpsertTodoCommandHandler(
        ITodoListRepository repository,
        ILogger<UpsertTodoCommandHandler> logger) : IRequestHandler<UpsertTodoCommand, Result<UpsertTodoItemDtoResult>>
    {
        public async Task<Result<UpsertTodoItemDtoResult>> Handle(UpsertTodoCommand request, CancellationToken cancellationToken)
        {
            var todoItemDto = request.Item;
            TodoItemMapper.EnsureId(todoItemDto);

            var wasCreated = await repository.UpsertAsync(todoItemDto, request.UserName, cancellationToken);

            logger.LogInformation(
                "{Action} todo item {TodoId} for user {UserName}",
                wasCreated ? "Created" : "Updated",
                todoItemDto.Id,
                request.UserName);

            return Result<UpsertTodoItemDtoResult>.Success(new UpsertTodoItemDtoResult
            {
                Item = todoItemDto,
                WasCreated = wasCreated
            });
        }
    }
}
