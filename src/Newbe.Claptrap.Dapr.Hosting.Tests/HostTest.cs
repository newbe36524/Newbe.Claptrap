using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Dapr.Hosting.Tests
{
    public class HostTest
    {
        [TestCase("sqlite")]
        [TestCase("mongodb")]
        [TestCase("mysql")]
        [TestCase("postgresql")]
        [TestCase("multiple_db")]
        public async Task BuildHost(string jsonFileName)
        {
            var hostBuilder = new HostBuilder();
            var host = hostBuilder
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile($"configs/load_db_config/{jsonFileName}.json");
                })
                .UseServiceProviderFactory(context =>
                {
                    return new AutofacServiceProviderFactory(builder =>
                    {
                        var loggerFactory = new ServiceCollection()
                            .AddLogging(logging =>
                            {
                                logging.SetMinimumLevel(LogLevel.Trace);
                                logging.AddConsole();
                            })
                            .BuildServiceProvider()
                            .GetRequiredService<ILoggerFactory>();
                        var claptrapBootstrapper =
                            (AutofacClaptrapBootstrapper) new AutofacClaptrapBootstrapperBuilder(loggerFactory)
                                .ScanClaptrapModule()
                                .AddConfiguration(context.Configuration)
                                .ScanClaptrapDesigns(new[]
                                {
                                    typeof(IAccount),
                                    typeof(Account),
                                    typeof(IAccountBalanceMinion),
                                    typeof(AccountBalanceMinion)
                                })
                                .Build();
                        claptrapBootstrapper.Boot(builder);
                    });
                })
                .ConfigureServices((_, collection) => { collection.AddClaptrapServerOptions(); })
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }
    }
}