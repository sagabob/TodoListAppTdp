using MediatR;

namespace TodoListApi.Application.Commands;

public record DeleteTodoCommand(Guid Id) : IRequest<Result>;
