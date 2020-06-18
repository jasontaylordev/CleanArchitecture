using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            ApplyMappingsFromAssembly(assembly);
            RegisterReverseMappings(assembly);
            RegisterCustomMappings(assembly);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") 
                    ?? type.GetInterface("IMapFrom`1").GetMethod("Mapping");
                
                methodInfo?.Invoke(instance, new object[] { this });

            }
        }

        /// <summary>
        /// Load all types that implement interface <see cref="IMapTo{T}"/>
        /// and create a map between them and {T}
        /// </summary>
        private void RegisterReverseMappings(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapTo`1").GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }

        /// Custom Mapping implementation
        /// https://docs.automapper.org/en/stable/Configuration.html
        /// Create mapping using configuration
        private void RegisterCustomMappings(Assembly assembly)
        {
            var maps = assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces()
                    .Any(i => typeof(IHaveCustomMappings).IsAssignableFrom(t)))
                .Select(t => (IHaveCustomMappings)Activator.CreateInstance(t))
                .ToArray();

            if (maps != null)
            {
                foreach (var map in maps)
                    map?.CreateMappings(AutoMapperConfig.MapperConfigurationExpression);

            }

        }
    }
}