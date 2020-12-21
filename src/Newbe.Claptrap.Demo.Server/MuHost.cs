using System.IO;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Demo.Server.Services;
using NLog.Web;

namespace Newbe.Claptrap.Demo.Server
{
    public class MuHost : IHostedService
    {
        private readonly IHost _host;

        public MuHost()
        {
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.AddJsonFile(Path.Combine("configs", "appsettings.json"))
                .AddEnvironmentVariables()
                .Build();
            var testConsoleOptions = new TestConsoleOptions();
            config.Bind(nameof(TestConsoleOptions), testConsoleOptions);

            _host = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    var databaseType = testConsoleOptions.DatabaseType;
                    const RelationLocatorStrategy strategy = RelationLocatorStrategy.SharedTable;
                    configurationBuilder
                        .AddJsonFile("configs/appsettings.json")
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.json".ToLower())
                        .AddJsonFile($"configs/db_configs/claptrap.{databaseType:G}.{strategy:G}.json".ToLower());

                    configurationBuilder.AddEnvironmentVariables();
                })
                .UseClaptrap(builder => { builder.ScanClaptrapDesigns(new[] {typeof(AccountGrain).Assembly}); })
                .UseOrleansClaptrap()
                // .UseOrleans(builder => { builder.UseDashboard(options => options.Port = 9000); })
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var metricsRoot = _host.Services.GetRequiredService<IMetricsRoot>();
            ClaptrapMetrics.MetricsRoot = metricsRoot;
            return _host.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _host.StopAsync(cancellationToken);
        }
    }
}