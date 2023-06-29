using System.Reflection;

namespace CleanArchitecture.WebUI.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointType = typeof(IEndpoint);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetExportedTypes()
            .Where(t => t.IsAbstract == false &&
                        t.GetInterfaces().Contains(endpointType));

        foreach (var type in endpointTypes)
        {
            if (Activator.CreateInstance(type) is IEndpoint instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }

    public static WebApplication MapEndpointGroups(this WebApplication app)
    {
        var endpointGroupType = typeof(EndpointGroupBase);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType));

        foreach (var type in endpointGroupTypes)
        {
            if (Activator.CreateInstance(type) is EndpointGroupBase instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}
