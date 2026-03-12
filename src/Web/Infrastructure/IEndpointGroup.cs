namespace CleanArchitecture.Web.Infrastructure;

/// <summary>
/// Defines a group of related Minimal API endpoints.
/// Any class in the assembly that implements this interface is automatically discovered at
/// compile time and registered as a route group at <c>/api/{ClassName}</c> with a matching
/// OpenAPI tag.
/// </summary>
public interface IEndpointGroup
{
    static abstract void Map(RouteGroupBuilder groupBuilder);
}
