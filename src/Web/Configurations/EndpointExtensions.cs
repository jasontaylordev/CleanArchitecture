
using System.Reflection;

namespace CleanArchitecture.Web.Configurations;

public static class EndpointExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpointGroupBaseType = typeof(EndpointGroupBase);

        // Find all non-abstract classes that inherit from EndpointGroupBase
        var endpointGroups = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && t.IsSubclassOf(endpointGroupBaseType))
            .Select(Activator.CreateInstance)
            .Cast<EndpointGroupBase>();

        // Call Map() on each one
        foreach (var group in endpointGroups)
        {
            group.Map(app);
        }
    }
}
