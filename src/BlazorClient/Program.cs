using BlazorClient.ApiClients;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("BlazorWebApplication1.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorWebApplication1.ServerAPI"));
  
            builder.Services.AddTransient(factory => new WeatherForecastClient(factory.GetRequiredService<HttpClient>()));
            builder.Services.AddTransient(factory => new TodoListsClient(factory.GetRequiredService<HttpClient>()));
            builder.Services.AddTransient(factory => new TodoItemsClient(factory.GetRequiredService<HttpClient>()));

            builder.Services.AddApiAuthorization();

            await builder.Build().RunAsync();
        }
    }
}
