using System.Reflection;

namespace CleanArchitecture.Web.Infrastructure;

public static class WebApplicationExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        var endpointGroupType = typeof(EndpointGroupBase);
        var assembly = Assembly.GetExecutingAssembly();

        var endpointGroupTypes = assembly.GetExportedTypes()
            .Where(t => t.IsSubclassOf(endpointGroupType) && !t.IsAbstract);

        foreach (var type in endpointGroupTypes)
        {
            services.AddTransient(endpointGroupType, type);
        }

        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroups = app.Services.GetServices<EndpointGroupBase>();

        foreach (var instance in endpointGroups)
        {
            instance.Map(app.MapGroup(instance));
        }

        return app;
    }

    private static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
    {
        var groupName = group.GroupName ?? group.GetType().Name;

        return app
            .MapGroup($"/api/{groupName}")
            .WithGroupName(groupName)
            .WithTags(groupName);
    }
}
