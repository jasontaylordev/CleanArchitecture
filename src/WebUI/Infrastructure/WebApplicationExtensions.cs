using System.Reflection;

namespace CleanArchitecture.WebUI.Infrastructure;

public static class WebApplicationExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endPointType = typeof(IEndpoint);

        var assembly = Assembly.GetExecutingAssembly();

        var endpointTypes = assembly.GetExportedTypes()
            .Where(t => t.IsAbstract == false &&
                        t.GetInterfaces().Contains(endPointType));

        foreach (var type in endpointTypes)
        {
            if (Activator.CreateInstance(type) is IEndpoint instance)
            {
                instance.Map(app);
            }
        }

        return app;
    }
}
