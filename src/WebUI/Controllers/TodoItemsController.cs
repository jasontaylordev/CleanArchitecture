using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItem;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CleanArchitecture.WebUI.Controllers
{
    [Authorize]
    public class TodoItemsController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<TodoItemsListVm>> GetAll()
        {
            return await Mediator.Send(new GetTodoItemsListQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemVm>> Get(long id)
        {
            return await Mediator.Send(new GetTodoItemQuery { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<long>> Create(CreateTodoItemCommand command)
        {
            var id = await Mediator.Send(command);

            return Created(nameof(Get), id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(long id, UpdateTodoItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            await Mediator.Send(new DeleteTodoItemCommand { Id = id });

            return NoContent();
        }
    }
}
