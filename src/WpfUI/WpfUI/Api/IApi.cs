using System.Net.Http;

namespace WpfUI.Api
{
    public interface IApi
    {
        HttpClient Client { get; }
    }
}