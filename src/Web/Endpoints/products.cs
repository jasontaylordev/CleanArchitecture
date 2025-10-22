using CleanArchitecture.Application.Products.Commands;
using CleanArchitecture.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CleanArchitecture.Web.Endpoints
{
    public class ProductsEndpoints : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            Console.WriteLine(">>> Mapping ProductsEndpoints...");
            app.MapGroup(this)
                .WithTags("Products")
               .MapGet(GetProducts)
               .MapPost(CreateProduct);
        }

        public async Task<IResult> GetProducts(IMediator mediator)
        {
            var products = await mediator.Send(new GetProductsQuery());
            return Results.Ok(products);
        }

        public async Task<IResult> CreateProduct(CreateProductCommand command, IMediator mediator)
        {
            var id = await mediator.Send(command);
            return Results.Ok(id);
        }
    }
}
