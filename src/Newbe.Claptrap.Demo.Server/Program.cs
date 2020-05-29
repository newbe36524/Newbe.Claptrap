using System;
using System.IO;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newtonsoft.Json;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        public static Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ExceptionObject);
            };
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(context =>
                {
                    var serviceProviderFactory = new AutofacServiceProviderFactory(
                        builder =>
                        {
                            var collection = new ServiceCollection().AddLogging(logging =>
                            {
                                logging.AddConsole();
                                logging.SetMinimumLevel(LogLevel.Debug);
                                logging.AddFilter((s, level) => s.StartsWith("Orleans") && level >= LogLevel.Warning);
                                logging.AddFilter((s, level) => s.Contains("Claptrap"));
                            });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, builder);
                            var claptrapBootstrapper = bootstrapperBuilder
                                .ScanClaptrapModule()
                                .ScanClaptrapDesigns(new[]
                                {
                                    typeof(AccountGrain).Assembly
                                })
                                // .UseSQLite(sqlite =>
                                //     sqlite
                                //         .AsEventStore(eventStore =>
                                //             eventStore.OneIdentityOneTable())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.OneIdentityOneTable())
                                // )
                                .AddConnectionString("claptrap",
                                    "Server=localhost;Database=claptrap;Uid=root;Pwd=claptrap;UseCompression=True;Pooling=True;Keepalive=10;MinimumPoolSize=10;MaximumPoolSize=50;")
                                .UseMySql(mysql =>
                                    mysql
                                        .AsEventStore(eventStore =>
                                            eventStore.SharedTable())
                                        .AsStateStore(stateStore =>
                                            stateStore.SharedTable())
                                )
                                .Build();
                            claptrapBootstrapper.Boot();
                            var store = claptrapBootstrapper.DumpDesignStore();
                            File.WriteAllText("design.json", JsonConvert.SerializeObject(store, Formatting.Indented));
                        });

                    return serviceProviderFactory;
                })
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        .UseLocalhostClustering()
                        .ConfigureApplicationParts(manager =>
                            manager.AddFromDependencyContext().WithReferences())
                        ;
                })
                .RunConsoleAsync();
        }
    }
}