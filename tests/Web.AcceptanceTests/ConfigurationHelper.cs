using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.Web.AcceptanceTests;

public static class ConfigurationHelper
{
    private readonly static IConfiguration _configuration;

    static ConfigurationHelper()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();
    }

    private static string? _baseUrl;

    public static string GetBaseUrl()
    {
        if (_baseUrl == null)
        {
            _baseUrl = _configuration["BaseUrl"];

            ArgumentNullException.ThrowIfNull(_baseUrl);

            _baseUrl = _baseUrl.TrimEnd('/');
        }

        return _baseUrl;
    }
}
