using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Items.Queries.GetItem;

// [Authorize]
public record GetItemsQuery : IRequest<ItemDto>
{
    public string? ItemNumber { get; set; }
}

public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, ItemDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetItemsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ItemDto?> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        var result = await _context.ItemMaster
                .AsNoTracking()
                .Where(i => i.ItemNumber == request.ItemNumber)
                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

        return result;
    }
}
