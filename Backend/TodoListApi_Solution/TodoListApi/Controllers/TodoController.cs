using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApi.Application.Commands;
using TodoListApi.Application.Requests;
using TodoListApi.Models;

namespace TodoListApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TodoController(IMediator mediator) : ControllerBase
{
    [HttpGet("all")]
    public async Task<ActionResult<TodoList>> GetAll(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;

        var todoList = await mediator.Send(new GetTodoListRequest(email), cancellationToken);

        return Ok(todoList.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoItemDto>> GetTodoById(Guid id, CancellationToken cancellationToken)
    {
        var todoDto = await mediator.Send(new GetTodoItemByIdRequest(id), cancellationToken);

        if (!todoDto.IsSuccess) return NotFound();

        return Ok(todoDto.Value);
    }

    [HttpPost]
    public async Task<ActionResult<TodoItemDto>> UpsertTodo([FromBody] TodoItemDto todoItemDto, CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email)!;

        var result = await mediator.Send(new UpsertTodoCommand(todoItemDto, email), cancellationToken);

        var outputItem = result.Value!;

        if (outputItem.WasCreated) return CreatedAtAction(nameof(GetTodoById), new { id = outputItem.Item.Id }, outputItem.Item);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTodo(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteTodoCommand(id), cancellationToken);

        if (!result.IsSuccess) return NotFound();

        return NoContent();
    }
}
