
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Products.Commands;
using CleanArchitecture.Application.Products.Queries;
using CleanArchitecture.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CleanArchitecture.Web.Pages.Products
{
    public class ProductsModel : PageModel
    {
        private readonly ISender _mediator;

        public ProductsModel(ISender mediator)
        {
            _mediator = mediator;
        }

        // List of products to display
        public List<ProductDto> Products { get; set; } = new();

        // Bind form data to this command object
        [BindProperty]
        public CreateProductCommand NewProduct { get; set; } = new();

        // Runs when the page loads
        public async Task OnGet()
        {
            Products = await _mediator.Send(new GetProductsQuery());
        }

        // Runs when the form is submitted
        public async Task<IActionResult> OnPostCreate()
        {
            if (!ModelState.IsValid)
            {
                // Reload product list if form validation fails
                await OnGet();
                return Page();
            }

            await _mediator.Send(NewProduct);

            // Redirect to GET to avoid resubmitting the form if the user refreshes
            return RedirectToPage();
        }
    }
}

