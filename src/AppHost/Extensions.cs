internal static class AspireExtensions
{
    public static IResourceBuilder<T> WithAspNetCoreEnvironment<T>(this IResourceBuilder<T> builder) 
        where T : IResourceWithEnvironment
    {
        builder.WithEnvironment(context =>
        {
            // Use the AppHost's environment if set. Otherwise default to Development
            // when running locally and Production when publishing or deploying.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? (context.ExecutionContext.IsRunMode ? "Development" : "Production");

            context.EnvironmentVariables["ASPNETCORE_ENVIRONMENT"] = environment;
        });

        return builder;
    }
}