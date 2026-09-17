using System.Diagnostics.CodeAnalysis;

namespace CleanArchitecture.Web.Infrastructure;

/// <summary>
/// Extends <see cref="IEndpointRouteBuilder"/> with convenience overloads used inside
/// <see cref="IEndpointGroup.Map"/>. Each method wraps the standard ASP.NET Core
/// <c>Map{Verb}</c> call and automatically derives the endpoint name from the handler's
/// method name, which becomes the OpenAPI <c>operationId</c> and is used for typed
/// client generation (e.g. <c>nswag</c>).
/// <para>
/// <c>pattern</c> is optional for GET and POST (collection-level operations that typically
/// have no route parameter) but required for PUT, PATCH, and DELETE (resource-level
/// operations that almost always target a specific item by ID, e.g. <c>"{id}"</c>).
/// </para>
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <inheritdoc cref="EndpointRouteBuilderExtensions"/>
    public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapGet(pattern, handler)
              .WithName(handler.Method.Name);
    }

    /// <inheritdoc cref="EndpointRouteBuilderExtensions"/>
    public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPost(pattern, handler)
            .WithName(handler.Method.Name);
    }

    /// <inheritdoc cref="EndpointRouteBuilderExtensions"/>
    public static RouteHandlerBuilder MapPut(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPut(pattern, handler)
            .WithName(handler.Method.Name);
    }

    /// <inheritdoc cref="EndpointRouteBuilderExtensions"/>
    public static RouteHandlerBuilder MapPatch(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapPatch(pattern, handler)
            .WithName(handler.Method.Name);
    }

    /// <inheritdoc cref="EndpointRouteBuilderExtensions"/>
    public static RouteHandlerBuilder MapDelete(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern)
    {
        Guard.Against.AnonymousMethod(handler);

        return builder.MapDelete(pattern, handler)
            .WithName(handler.Method.Name);
    }
}
