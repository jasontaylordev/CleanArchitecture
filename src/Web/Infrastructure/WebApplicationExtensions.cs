using System.Reflection;

namespace CleanArchitecture.Web.Infrastructure;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Discovers all <see cref="IEndpointGroup"/> implementations in <paramref name="assembly"/>
    /// and registers each as a route group at <c>/api/{ClassName}</c> with a matching OpenAPI tag.
    /// </summary>
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                     && t.IsAssignableTo(typeof(IEndpointGroup)));

        foreach (var type in endpointGroupTypes)
        {
            var groupName = type.Name;
            var group = app.MapGroup($"/api/{groupName}").WithTags(groupName);
            type.GetMethod(nameof(IEndpointGroup.Map))!.Invoke(null, [group]);
        }

        return app;
    }
}
