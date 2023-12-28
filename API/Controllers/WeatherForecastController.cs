using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
public class TodoItemController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ISender _sender;

    public TodoItemController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost(nameof(Create))]
    public async Task<int> Create(CreateTodoItemCommand command)
    {
        return await _sender.Send(command);
    }
}
