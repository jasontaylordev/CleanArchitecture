using Microsoft.AspNetCore.Authorization;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace CleanArchitecture.Web.Infrastructure;

public class ApiExceptionOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;

        operation.Responses.TryAdd("400", new OpenApiResponse
        {
            Description = "Bad Request"
        });

        var aspNetCoreContext = context as AspNetCoreOperationProcessorContext;
        var requiresAuth = aspNetCoreContext?.ApiDescription.ActionDescriptor.EndpointMetadata
            .Any(m => m is IAuthorizeData) == true;

        if (requiresAuth)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse
            {
                Description = "Unauthorized"
            });

            operation.Responses.TryAdd("403", new OpenApiResponse
            {
                Description = "Forbidden"
            });
        }

        return true;
    }
}
