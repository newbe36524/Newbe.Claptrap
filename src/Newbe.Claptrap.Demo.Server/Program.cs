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
                            });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, builder);
                            const string mysqlConnectionString =
                                "Server=localhost;Database=claptrap;Uid=root;Pwd=claptrap;Pooling=True;";
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
                                    mysqlConnectionString)
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
                        .ConfigureLogging(logging =>
                        {
                            logging.SetMinimumLevel(LogLevel.Debug);
                            logging.AddFilter(
                                (s, level) => s.StartsWith("Microsoft.Orleans") && level >= LogLevel.Error);
                            logging.AddFilter((s, level) => s.Contains("Claptrap") && level >= LogLevel.Information);
                        })
                        .ConfigureApplicationParts(manager =>
                            manager.AddFromDependencyContext().WithReferences())
                        // .UseDashboard(options => options.Port = 9000)
                        ;
                })
                .RunConsoleAsync();
        }
    }
}