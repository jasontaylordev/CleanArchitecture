using CleanArchitecture.WebUI.Filters;

namespace CleanArchitecture.WebUI.Infrastructure;

public abstract class EndpointGroupBase
{
    private string? _groupName;
    private RouteGroupBuilder? _group;

    public abstract void Map(WebApplication app);

    protected RouteGroupBuilder MapGroup(WebApplication app, string groupName)
    {
        _groupName = groupName;

        return _group = app
            .MapGroup($"/api/{_groupName}")
            .WithGroupName(_groupName)
            .WithTags(_groupName)
            .RequireAuthorization()
            .WithOpenApi()
            .AddEndpointFilter<ApiExceptionFilter>();
    }

    public RouteHandlerBuilder MapGet(Delegate handler, string prefix = "")
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        return MapGet(handler.Method.Name, prefix, handler);
    }

    protected RouteHandlerBuilder MapGet(string name, Delegate handler)
    {
        return MapGet(name, "", handler);
    }

    protected RouteHandlerBuilder MapGet(string name, string prefix, Delegate handler)
    {
        return _group!.MapGet(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    public RouteHandlerBuilder MapPost(Delegate handler, string prefix = "")
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        return MapPost(handler.Method.Name, prefix, handler);
    }

    protected RouteHandlerBuilder MapPost(string name, Delegate handler)
    {
        return MapPost(name, "", handler);
    }

    protected RouteHandlerBuilder MapPost(string name, string prefix, Delegate handler)
    {
        return _group!.MapPost(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    protected RouteHandlerBuilder MapPut(Delegate handler, string prefix)
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        return MapPut(handler.Method.Name, prefix, handler);
    }

    protected RouteHandlerBuilder MapPut(string name, string prefix, Delegate handler)
    {
        return _group!.MapPut(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    protected RouteHandlerBuilder MapDelete(Delegate handler, string prefix)
    {
        if (handler.Method.IsAnonymous())
        {
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
        }

        return MapDelete(handler.Method.Name, prefix, handler);
    }

    protected RouteHandlerBuilder MapDelete(string name, string prefix, Delegate handler)
    {
        return _group!.MapDelete(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    private string GetEndpointName(string name) => $"{_groupName}_{name}";
}
