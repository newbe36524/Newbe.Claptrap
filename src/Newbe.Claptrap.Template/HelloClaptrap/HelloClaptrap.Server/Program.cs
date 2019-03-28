using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HelloClaptrap.Implements;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.EventHub.DirectClient;
using Orleans;
using Orleans.Hosting;

namespace HelloClaptrap.Server
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
                    containerBuilder.RegisterModule<ClaptrapServerModule>();
                    containerBuilder.RegisterModule<DirectClientEventHubModule>();
                    
                    // Creating a new AutofacServiceProvider makes the container
                    // available to your app using the Microsoft IServiceProvider
                    // interface so you can use those abstractions rather than
                    // binding directly to Autofac.
                    var container = containerBuilder.Build();
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