using AntDesign.Pro.Layout;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HelloClaptrap.SimulatorWeb.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace HelloClaptrap.SimulatorWeb
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var services = builder.Services;
            services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            services.AddAntDesign();
            services.Configure<ProSettings>(builder.Configuration.GetSection("ProSettings"));
            services.AddRefitClient<IAuctionApi>()
                .ConfigureHttpClient(client => { client.BaseAddress = new Uri("http://localhost:38000"); });
            await builder.Build().RunAsync();
        }
    }
}