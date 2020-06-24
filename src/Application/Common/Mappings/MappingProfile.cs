using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Mappings
{
    /// <summary>
    /// Reference: https://stackoverflow.com/questions/47959135/automapper-open-closed-principle
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            var types = Assembly.GetExecutingAssembly().GetExportedTypes();
            ApplyMappingsFromAssembly(types);
            RegisterReverseMappings(types);
            RegisterCustomMappings(types);
        }

        private void ApplyMappingsFromAssembly(Type[] types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)
                              && !t.IsAbstract
                              && !t.IsInterface
                        select new
                        {
                            Source = i.GetGenericArguments()[0],
                            Destination = t
                        }).ToArray();

            foreach (var map in maps)
            {
                AutoMapperConfig.MapperConfigurationExpression?.CreateMap(map.Source, map.Destination);
            }
        }

        /// <summary>
        /// Load all types that implement interface <see cref="IMapTo{T}"/>
        /// and create a map between them and {T}
        /// </summary>
        private void RegisterReverseMappings(Type[] types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>)
                              && !t.IsAbstract
                              && !t.IsInterface
                        select new
                        {
                            Source = t,
                            Destination = i.GetGenericArguments()[0]
                        }).ToArray();

            foreach (var map in maps)
            {
                AutoMapperConfig.MapperConfigurationExpression?.CreateMap(map.Source, map.Destination);
            }
        }

        /// Custom Mapping implementation
        /// https://docs.automapper.org/en/stable/Configuration.html
        /// Create mapping using configuration
        private void RegisterCustomMappings(Type[] types)
        {
            var maps = (from t in types
                        from i in t.GetInterfaces()
                        where typeof(IHaveCustomMappings).IsAssignableFrom(t)
                              && !t.IsAbstract
                              && !t.IsInterface
                        select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

            if (maps != null)
            {
                foreach (var map in maps)
                    map?.CreateMappings(AutoMapperConfig.MapperConfigurationExpression);

            }

        }
    }
}