using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.StorageTestConsole.Services;
using Newbe.Claptrap.TestSuit;

namespace Newbe.Claptrap.StorageTestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.AddJsonFile(Path.Combine("configs", "appsettings.json"))
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var options = new TestConsoleOptions();
            config.Bind(nameof(TestConsoleOptions), options);

            const RelationLocatorStrategy strategy = RelationLocatorStrategy.SharedTable;
            var host = QuickSetupTestHelper.BuildHost(options.DatabaseType,
                strategy,
                Enumerable.Empty<string>(),
                containerBuilder =>
                {
                    containerBuilder.RegisterModule<StorageTestConsoleModule>();
                    containerBuilder.RegisterModule<StorageSetupModule>();
                }, services => { },
                hostBuilder =>
                {
                    hostBuilder.ConfigureServices(services =>
                    {
                        services.AddOptions<TestConsoleOptions>()
                            .Configure(consoleOptions => config.Bind(nameof(TestConsoleOptions), consoleOptions));
                    });
                });
            var serviceProvider = host.Services;
            await using var scope = serviceProvider.GetService<ILifetimeScope>();
            var logger = scope!.Resolve<ILogger<Program>>();
            try
            {
                var service = scope.ResolveKeyed<ITestJob>(options.Job);
                await service!.RunAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "error while storage test");
            }
        }
    }
}