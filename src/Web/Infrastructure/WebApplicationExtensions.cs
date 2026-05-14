using System.Reflection;

namespace CleanArchitecture.Web.Infrastructure;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Discovers all <see cref="IEndpointGroup"/> implementations in <paramref name="assembly"/>
    /// and registers each as a route group with a matching OpenAPI tag. The route prefix defaults
    /// to <c>/api/{ClassName}</c> but can be overridden via <see cref="IEndpointGroup.RoutePrefix"/>.
    /// </summary>
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                     && t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var type in endpointGroupTypes)
        {
            var groupName = type.Name;
            var routePrefix = type.GetProperty(nameof(IEndpointGroup.RoutePrefix))
                ?.GetValue(null) as string ?? $"/api/{groupName}";
            var group = app.MapGroup(routePrefix).WithTags(groupName);
            type.GetMethod(nameof(IEndpointGroup.Map))!.Invoke(null, [group]);
        }

        return app;
    }
}
