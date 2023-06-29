using CleanArchitecture.WebUI.Filters;

namespace CleanArchitecture.WebUI.Infrastructure;

public abstract class EndpointGroupBase
{
    private string? _groupName;
    private RouteGroupBuilder? _group;

    public abstract void Map(WebApplication app);

    protected void MapGroup(string groupName, WebApplication app)
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

    protected void MapGet(string name, Delegate handler)
    {
        MapGet(name, "", handler);
    }

    protected void MapGet(string name, string prefix, Delegate handler)
    {
        _group!.MapGet(prefix, handler)
            .WithName(GetEndpointName(name));
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

    protected void MapPut(string name, string prefix, Delegate handler)
    {
        _group!.MapPut(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    protected void MapDelete(string name, string prefix, Delegate handler)
    {
        _group!.MapDelete(prefix, handler)
            .WithName(GetEndpointName(name));
    }

    private string GetEndpointName(string name) => $"{_groupName}_{name}";
}
