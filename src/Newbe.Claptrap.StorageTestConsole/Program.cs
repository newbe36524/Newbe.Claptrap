using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
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
            var databaseType = DatabaseType.MongoDB;
            var strategy = RelationLocatorStrategy.SharedTable;
            var host = QuickSetupTestHelper.BuildHost(databaseType,
                strategy,
                Enumerable.Empty<string>(),
                builderAction: containerBuilder =>
                {
                    containerBuilder.RegisterModule<StorageTestConsoleModule>();
                    containerBuilder.RegisterModule<StorageSetupModule>();
                });
            var serviceProvider = host.Services;
            var logger = serviceProvider.GetService<ILogger<Program>>();
            try
            {
                var service = serviceProvider.GetService<IEventInsertTestService>();
                await service!.RunAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e, "error while storage test");
                Console.WriteLine(e);
            }
        }
    }
}