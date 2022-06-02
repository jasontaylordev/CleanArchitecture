using Microsoft.Extensions.Configuration;

namespace CleanArchitecture.WebUI.AcceptanceTests;
public static class ConfigurationHelper
{
    private readonly static IConfiguration _configuration;

    static ConfigurationHelper()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public static string GetBaseUrl()
    {
        return _configuration["BaseUrl"];
    }
}
