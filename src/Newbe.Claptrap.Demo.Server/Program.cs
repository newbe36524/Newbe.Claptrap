using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Impl.Bootstrapper;
using Newbe.Claptrap.Preview.StorageProvider.SQLite;
using Newtonsoft.Json;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var loggingCollection = new ServiceCollection();
            loggingCollection.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Debug);
            });
            var provider = loggingCollection.BuildServiceProvider();
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            var hostBuilder = new SiloHostBuilder();

            hostBuilder
                .UseLocalhostClustering()
                .UseServiceProviderFactory(collection =>
                {
                    collection.AddLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Debug);
                        logging.AddFilter((s, level) => s.StartsWith("Orleans") && level >= LogLevel.Warning);
                        logging.AddFilter((s, level) => s.Contains("Claptrap"));
                    });
                    var builder = new ContainerBuilder();

                    // Once you've registered everything in the ServiceCollection, call
                    // Populate to bring those registrations into Autofac. This is
                    // just like a foreach over the list of things in the collection
                    // to add them to Autofac.
                    builder.Populate(collection);

                    IClaptrapBootstrapperBuilder claptrapBootstrapperFactory =
                        new AutofacClaptrapBootstrapperBuilder(loggerFactory);
                    var claptrapBootstrapper = claptrapBootstrapperFactory
                        .AddAssemblies(new[]
                        {
                            typeof(Account).Assembly
                        })
                        .ConfigureGlobalClaptrapDesign(design =>
                        {
                            design.EventLoaderFactoryType = typeof(SQLiteEventStoreFactory);
                            design.EventSaverFactoryType = typeof(SQLiteEventStoreFactory);
                            design.StateLoaderFactoryType = typeof(SQLiteStateStoreFactory);
                            design.StateSaverFactoryType = typeof(SQLiteStateStoreFactory);
                        })
                        .SetCultureInfo(new CultureInfo("cn"))
                        .Build();
                    
                    claptrapBootstrapper.RegisterServices(builder);

                    var store = claptrapBootstrapper.DumpDesignStore();
                    File.WriteAllText("design.json", JsonConvert.SerializeObject(store, Formatting.Indented));

                    // Creating a new AutofacServiceProvider makes the container
                    // available to your app using the Microsoft IServiceProvider
                    // interface so you can use those abstractions rather than
                    // binding directly to Autofac.
                    var container = builder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                })
                .ConfigureApplicationParts(manager =>
                    manager.AddFromDependencyContext().WithReferences())
                .UseDashboard(options => options.Port = 9999)
                ;
            var siloHost = hostBuilder.Build();
            Console.WriteLine("server starting");
            await siloHost.StartAsync();
            Console.WriteLine("server started");

            Console.ReadLine();
        }
    }
}