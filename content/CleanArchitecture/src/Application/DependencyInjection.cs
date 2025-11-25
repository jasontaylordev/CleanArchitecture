using Cubido.Template.Application.Common.Behaviours;
using Cubido.Template.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddMediator(options =>
        {
            // Docs say it should be singleton for performance, but cannot consume scoped DbContext from singleton (would have to switch to DbContextFactory)
            // https://github.com/martinothamar/Mediator?tab=readme-ov-file#6-differences-from-mediatr
            options.ServiceLifetime = ServiceLifetime.Scoped;
            options.PipelineBehaviors = [
                typeof(AuthorizationBehaviour<,>),
                typeof(LoggingBehaviour<,>),
                typeof(PerformanceBehaviour<,>),
                typeof(ValidationBehaviour<,>),
                typeof(UnhandledExceptionBehaviour<,>),
            ];
        });

        int sqidLength = 7; // enough for int.MaxValue values
        builder.Configuration.GetSection("Sqids").Bind(SqidsOptionsFactory.Default);
        SqidsOptionsFactory.Configure<TodoItem>(sqidLength);
        SqidsOptionsFactory.Configure<TodoList>(sqidLength);
    }
}
