using NUnit.Framework;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.NUnitTests
{
    using static Testing;

    public class TestBase
    {
        
        [SetUp]
        public async Task TestSetUp()
        {
            await ResetState();
        }
    }
}
