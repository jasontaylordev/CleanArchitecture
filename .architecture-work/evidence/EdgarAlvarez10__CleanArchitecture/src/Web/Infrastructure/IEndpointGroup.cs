namespace CleanArchitecture.Web.Infrastructure;

/// <summary>
/// Defines a group of related Minimal API endpoints.
/// Implementations are automatically discovered and registered as a route group with a matching
/// OpenAPI tag. By default the route prefix is <c>/api/{ClassName}</c>; override
/// <see cref="RoutePrefix"/> to use a custom path, including nested resource paths such as
/// <c>/api/TodoLists/{todoListId}/TodoItems</c>.
/// </summary>
public interface IEndpointGroup
{
    /// <summary>
    /// The route prefix for this endpoint group.
    /// Defaults to <c>/api/{ClassName}</c>. Override to specify a custom or nested path.
    /// </summary>
    static virtual string? RoutePrefix => null;

    static abstract void Map(RouteGroupBuilder groupBuilder);
}
