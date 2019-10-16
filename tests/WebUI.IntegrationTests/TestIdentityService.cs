using CleanArchitecture.Application;
using System.Threading.Tasks;

namespace CleanArchitecture.WebUI.IntegrationTests
{
    public class TestIdentityService : IIdentityService
    {
        public Task<string> GetUserNameAsync(string userId)
        {
            return Task.FromResult("jason@clean-architecture");
        }
    }
}
