using CleanArchitecture.Application.Items.Queries.GetItem;
// using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

// [Authorize]
public class ItemsController : ApiControllerBase
{

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> Get(string id)
    {
        return await Mediator.Send(new GetItemsQuery { ItemNumber = id });
    }
}
