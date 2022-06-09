using AutoMapper;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Items.Queries.GetItem;

public class ItemDto : IMapFrom<ItemMaster>
{
    public string? ItemNumber { get; set; }
    public string? ItemTermType { get; set; }
    public string? ItemType { get; set; }
    public string? ItemDescription { get; set; }
    public int ItemProdLine { get; set; }
    public decimal ItemWeight { get; set; }

    // public void Mapping(Profile profile)
    // {
    //     profile.CreateMap<ItemMaster, ItemDto>()
    //         .ForMember(d => d.Priority, opt => opt.MapFrom(s => (int)s.Priority));
    // }
}
