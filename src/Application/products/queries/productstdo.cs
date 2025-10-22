using AutoMapper;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Products.Queries.GetProducts;

public class ProductDto
{
    public int Id { get; init; }

    public string Name { get; init; } = default!;

    public string? Description { get; init; }

    public decimal Price { get; init; }

    public string? ImageUrl { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
