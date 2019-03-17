using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
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
            var random = new Random();
            var sw = Stopwatch.StartNew();
            var tasks = Enumerable.Range(1, 1000).Select(x =>
            {
                var transferAccountBalance = client.GetGrain<ITransferAccountBalance>(x.ToString());
                return transferAccountBalance.Transfer(GetRandomAccountId(), GetRandomAccountId(), 1);
            });
            await Task.WhenAll(tasks);

            Console.WriteLine($"finished in {sw.ElapsedMilliseconds} ms");
            var sum = 0M;
            for (int i = 0; i < 10; i++)
            {
                var account = client.GetGrain<IAccount>(i.ToString());
                var balance = await account.GetBalance();
                sum += balance;
                Console.WriteLine($"balance for {i} is : {balance}");
            }

            Console.WriteLine($"total balance : {sum}");

            string GetRandomAccountId()
            {
                return random.Next(0, 500).ToString();
            }
        }
    }
}