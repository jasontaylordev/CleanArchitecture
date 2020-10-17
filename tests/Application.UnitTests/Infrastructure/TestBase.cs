using AutoMapper;
using CleanArchitecture.Application.Common.Mappings;
using NUnit.Framework;

namespace CleanArchitecture.Application.UnitTests.Infrastructure
{
    public class TestBase
    {
        protected IConfigurationProvider Configuration;
        protected IMapper Mapper;

        [SetUp]
        public virtual void Init()
        {
            // initialize the mapper so it behaves normally (nothing is mocked here)

            Configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            Mapper = Configuration.CreateMapper();
        }
    }
}
