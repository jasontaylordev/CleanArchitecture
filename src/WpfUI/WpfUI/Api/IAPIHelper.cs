using System.Net.Http;

namespace WpfUI.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
    }
}