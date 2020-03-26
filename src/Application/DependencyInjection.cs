using AutoMapper;
using CleanArchitecture.Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddFluentValidation(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            return services;
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly assembly)
        {
            var validatorType = typeof(IValidator<>);

            var validatorTypes = assembly
                .GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == validatorType))
                .ToList();

            foreach (var validator in validatorTypes)
            {
                var requestType = validator.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .First();

                var validatorInterface = validatorType
                    .MakeGenericType(requestType);

                services.AddTransient(validatorInterface, validator);
            }

            return services;
        }
    }
}
