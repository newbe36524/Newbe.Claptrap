using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
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

            var accountId = "123";
            var account = client.GetGrain<IAccount>(accountId);
            var balance = await account.GetBalance();
            Console.WriteLine(balance);
            var sw = Stopwatch.StartNew();
            const int times = 1;
            await Task.WhenAll(Enumerable.Range(0, times)
                .Select(i => account.TransferIn(100, Guid.NewGuid().ToString())));
            Console.WriteLine(await account.GetBalance());
            sw.Stop();
            Console.WriteLine($"cost time {sw.ElapsedMilliseconds} ms in {times}");

            var accountMinion = client.GetGrain<IAccountBalanceMinion>(accountId);
            sw.Restart();
            var balanceInMinion = await accountMinion.GetBalance();
            sw.Stop();
            Console.WriteLine($"balance in minion is {balanceInMinion}, cost time {sw.ElapsedMilliseconds} ms");
            // var random = new Random();
            // var sw = Stopwatch.StartNew();
            // var round = 0;
            // while (true)
            // {
            //     round++;
            //     Thread.Sleep(500);
            //     Console.WriteLine($"round {round} now");
            //     sw.Restart();
            //     const int count = 1000;
            //     var tasks = Enumerable.Range(round * count, count).Select(x =>
            //     {
            //         var transferAccountBalance = client.GetGrain<ITransferAccountBalance>(x.ToString());
            //         return transferAccountBalance.Transfer(GetRandomAccountId(), GetRandomAccountId(), 1);
            //     });
            //     await Task.WhenAll(tasks);
            //
            //     Console.WriteLine($"finished in {sw.ElapsedMilliseconds} ms");
            // }

            // string GetRandomAccountId()
            // {
            //     return random.Next(0, 500).ToString();
            // }
            client.Dispose();
        }
    }
}