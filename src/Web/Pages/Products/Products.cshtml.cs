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

        public List<ProductDto> Products { get; set; } = new();

        [BindProperty]
        public CreateProductCommand? NewProduct { get; set; }

        [BindProperty]
        public UpdateProductCommand? UpdateProduct { get; set; }

        [BindProperty]
        public string? DeleteName { get; set; }

        public async Task OnGet()
        {
            Products = await _mediator.Send(new GetProductsQuery());
        }

        public async Task<IActionResult> OnPostCreate()
        {
            if (NewProduct == null || string.IsNullOrWhiteSpace(NewProduct.Name) || string.IsNullOrWhiteSpace(NewProduct.Description))
            {
                await OnGet();
                return Page();
            }

            await _mediator.Send(NewProduct);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdate()
        {
            if (UpdateProduct == null || string.IsNullOrWhiteSpace(UpdateProduct.Name) || string.IsNullOrWhiteSpace(UpdateProduct.Description))
            {
                await OnGet();
                return Page();
            }

            await _mediator.Send(UpdateProduct);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            if (string.IsNullOrWhiteSpace(DeleteName))
            {
                await OnGet();
                return Page();
            }

            await _mediator.Send(new DeleteProductCommand { Name = DeleteName });
            return RedirectToPage();
        }
    }
}
