using System.Reflection;

namespace CleanArchitecture.Web.Infrastructure;

public static class MethodInfoExtensions
{
    // Compiler-generated anonymous methods (lambdas, local functions) contain '<' and '>' in their names.
    private static readonly char[] AnonymousMethodChars = ['<', '>'];

    /// <summary>
    /// Returns <see langword="true"/> if the method was compiler-generated from an anonymous delegate or lambda
    /// (i.e. it has no stable, human-readable name that can be used as an OpenAPI <c>operationId</c>).
    /// </summary>
    public static bool IsAnonymous(this MethodInfo method) =>
        method.Name.Any(AnonymousMethodChars.Contains);

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="input"/> is an anonymous handler.
    /// Endpoint handlers must be named methods so that a meaningful <c>operationId</c> can be derived
    /// from <see cref="MethodInfo.Name"/> for use in OpenAPI and typed client generation.
    /// </summary>
    public static void AnonymousMethod(this IGuardClause guardClause, Delegate input)
    {
        if (input.Method.IsAnonymous())
            throw new ArgumentException("The endpoint name must be specified when using anonymous handlers.");
    }
}