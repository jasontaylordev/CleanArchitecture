using System.Threading.Tasks;

namespace CleanArchitecture.Application
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);
    }
}
