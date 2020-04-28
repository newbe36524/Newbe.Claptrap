using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;
using Newbe.Claptrap.StorageProvider.SQLite.Module;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new SiloHostBuilder();

            hostBuilder
                .UseLocalhostClustering()
                .UseServiceProviderFactory(collection =>
                {
                    collection.AddLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Debug);
                    });
                    var builder = new ContainerBuilder();

                    // Once you've registered everything in the ServiceCollection, call
                    // Populate to bring those registrations into Autofac. This is
                    // just like a foreach over the list of things in the collection
                    // to add them to Autofac.
                    builder.Populate(collection);

                    var buildServiceProvider = collection.BuildServiceProvider();
                    var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                    var claptrapBootstrapperFactory = new AutofacClaptrapBootstrapperFactory(loggerFactory);
                    var claptrapBootstrapper = claptrapBootstrapperFactory.Create(new[]
                    {
                        typeof(Account).Assembly
                    });
                    claptrapBootstrapper.RegisterServices(builder);
                    builder.RegisterModule<SQLiteStorageModule>();

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