using CleanArchitecture.WebUI.Filters;

namespace CleanArchitecture.WebUI.Infrastructure;

public abstract class EndpointGroupBase
{
    private string? _groupName;
    private RouteGroupBuilder? _group;

    public abstract void Map(WebApplication app);

    protected void MapGroup(WebApplication app, string groupName)
    {
        _groupName = groupName;

        _group = app
            .MapGroup($"/api/{_groupName}")
            .WithGroupName(_groupName)
            .WithTags(_groupName)
            .RequireAuthorization()
            .WithOpenApi()
            .AddEndpointFilter<ApiExceptionFilter>();
    }

    public void MapGet(Delegate handler, string prefix = "")
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        MapGet(handler.Method.Name, prefix, handler);
    }

    protected void MapGet(string name, Delegate handler)
    {
        MapGet(name, "", handler);
    }

    protected void MapGet(string name, string prefix, Delegate handler)
    {
        _group!.MapGet(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    public void MapPost(Delegate handler, string prefix = "")
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        MapPost(handler.Method.Name, prefix, handler);
    }

    protected void MapPost(string name, Delegate handler)
    {
        MapPost(name, "", handler);
    }

    protected void MapPost(string name, string prefix, Delegate handler)
    {
        _group!.MapPost(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    protected void MapPut(Delegate handler, string prefix)
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        MapPut(handler.Method.Name, prefix, handler);
    }

    protected void MapPut(string name, string prefix, Delegate handler)
    {
        _group!.MapPut(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    protected void MapDelete(Delegate handler, string prefix)
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        MapDelete(handler.Method.Name, prefix, handler);
    }

    protected void MapDelete(string name, string prefix, Delegate handler)
    {
        _group!.MapDelete(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    private string GetEndpointName(string name) => $"{_groupName}_{name}";
}
