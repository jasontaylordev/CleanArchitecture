namespace CleanArchitecture.Web.Infrastructure;

public abstract class EndpointGroupBase(ISender sender)
{
    protected readonly ISender _sender = sender;
    public abstract void Map(WebApplication app);
}
