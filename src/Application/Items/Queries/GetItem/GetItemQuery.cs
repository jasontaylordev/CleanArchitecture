using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<GetItemsQueryHandler> _logger;

    public GetItemsQueryHandler(IApplicationDbContext context, IMapper mapper, ILogger<GetItemsQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ItemDto?> Handle(GetItemsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start item query");
        var result = await _context.ItemMaster
                .AsNoTracking()
                .Where(i => i.ItemNumber == request.ItemNumber)
                .ProjectTo<ItemDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

        _logger.LogInformation("End item query");
        return result;
    }
}
