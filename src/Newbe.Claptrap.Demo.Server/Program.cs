using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.EventHub.Memory;
using Newbe.Claptrap.EventStore.Memory;
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
                    var containerBuilder = new ContainerBuilder();

                    // Once you've registered everything in the ServiceCollection, call
                    // Populate to bring those registrations into Autofac. This is
                    // just like a foreach over the list of things in the collection
                    // to add them to Autofac.
                    containerBuilder.Populate(collection);

                    containerBuilder.RegisterModule<DemoModule>();
                    containerBuilder.RegisterModule<ClaptrapModule>();
                    // Creating a new AutofacServiceProvider makes the container
                    // available to your app using the Microsoft IServiceProvider
                    // interface so you can use those abstractions rather than
                    // binding directly to Autofac.
                    var container = containerBuilder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                })
                .AddStartupTask(async (provider, token) =>
                {
                    var grainFactory = provider.GetService<IGrainFactory>();
                    var account = grainFactory.GetGrain<IAccount>("666");
                    var balance = await account.GetBalance();
                    Console.WriteLine($"balance now is {balance}");
                    await Task.WhenAll(Enumerable.Range(0, 100).Select(i => account.AddBalance(2)));
                    balance = await account.GetBalance();
                    Console.WriteLine($"balance now is {balance}");
                })
                ;
            var siloHost = hostBuilder.Build();
            Console.WriteLine("server starting");
            await siloHost.StartAsync();
            Console.WriteLine("server started");

            await RunClient();
            Console.ReadLine();
        }

        private static async Task RunClient()
        {
            var clientBuilder = new ClientBuilder();
            clientBuilder.UseLocalhostClustering();
            var clusterClient = clientBuilder.Build();
            await clusterClient.Connect(exception => Task.FromResult(true));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<EventHudReceiverService>()
                .As<IEventHudReceiverService>()
                .SingleInstance();
            containerBuilder.RegisterType<EventHubManager>()
                .As<IEventHubManager>()
                .SingleInstance();
            containerBuilder.Register(context =>
                    clusterClient)
                .ExternallyOwned()
                .As<IClusterClient>()
                .As<IGrainFactory>()
                .SingleInstance();
            var container = containerBuilder.Build();
            var service = container.Resolve<IEventHudReceiverService>();
            await service.Start();
        }

        public interface IEventHudReceiverService
        {
            Task Start();
        }

        private class EventHudReceiverService : IEventHudReceiverService
        {
            public Task Start()
            {
                throw new NotImplementedException();
            }
        }
    }
}