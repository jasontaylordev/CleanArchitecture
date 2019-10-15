using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsList;

namespace CleanArchitecture.WebUI.Controllers
{
    [Authorize]
    public class TodoItemsController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<TodoItemsListVm>> Get()
        {
            return await Mediator.Send(new GetTodoItemsListQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemVm>> Get(long id)
        {
            return await Mediator.Send(new GetTodoItemQuery { Id = id });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<long>> Create(CreateTodoItemCommand command)
        {
            var id = await Mediator.Send(command);

            return Created(nameof(Get), id);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Update(UpdateTodoItemCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(DeleteTodoItemCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}
