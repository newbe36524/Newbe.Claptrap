using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Newbe.Claptrap.Demo.Interfaces;
using Orleans;

namespace Newbe.Claptrap.Demo.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientBuilder = new ClientBuilder();
            clientBuilder
                .UseLocalhostClustering()
                .UseServiceProviderFactory(collection =>
                {
                    var containerBuilder = new ContainerBuilder();

                    // Once you've registered everything in the ServiceCollection, call
                    // Populate to bring those registrations into Autofac. This is
                    // just like a foreach over the list of things in the collection
                    // to add them to Autofac.
                    containerBuilder.Populate(collection);

                    // Creating a new AutofacServiceProvider makes the container
                    // available to your app using the Microsoft IServiceProvider
                    // interface so you can use those abstractions rather than
                    // binding directly to Autofac.
                    var container = containerBuilder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                });
            var client = clientBuilder.Build();
            Console.WriteLine("start to connect");
            await client.Connect(exception => Task.FromResult(true));
            Console.WriteLine("connected");
            var account = client.GetGrain<IAccount>("666");
            var balance = await account.GetBalance();
            Console.WriteLine($"balance now is {balance}");
            for (int i = 0; i < 10; i++)
            {
                await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => account.AddBalance(2)));
                Console.WriteLine("start to get balance");
                balance = await account.GetBalance();
                Console.WriteLine($"balance now is {balance}");
            }
      
        }
    }
}