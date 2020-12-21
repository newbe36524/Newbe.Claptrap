using System.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;
using Newbe.Claptrap;
using Orleans;
using Orleans.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseClaptrapOrleansHost(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                    .UseOrleans((context, builder) =>
                    {
                        var claptrapOptions = new ClaptrapServerOptions();
                        var config =
                            context.Configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
                        config.Bind(claptrapOptions);
                        var claptrapOptionsOrleans = claptrapOptions.Orleans;
                        var hostname = claptrapOptionsOrleans.Hostname ?? "localhost";
                        if (!IPAddress.TryParse(hostname, out var ip))
                        {
                            ip = Dns.GetHostEntry(hostname).AddressList.First();
                        }

                        const int defaultGatewayPort = 30000;
                        const int defaultSiloPort = 11111;
                        var gatewayPort = claptrapOptionsOrleans.GatewayPort
                                          ?? defaultGatewayPort;
                        var siloPort = claptrapOptionsOrleans.SiloPort
                                       ?? defaultSiloPort;
                        builder
                            .ConfigureDefaults()
                            .UseLocalhostClustering()
                            .ConfigureEndpoints(ip, siloPort, gatewayPort)
                            .ConfigureApplicationParts(manager =>
                                manager.AddFromDependencyContext().WithReferences());
                    })
                ;
        }
    }
}