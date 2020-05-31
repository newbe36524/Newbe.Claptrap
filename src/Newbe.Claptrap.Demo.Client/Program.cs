using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var rd = new Random();

            var ids = Enumerable.Range(1, 1);
            var sw = Stopwatch.StartNew();
            await Task.WhenAll(ids.SelectMany(x => RunOneAccount(x.ToString())));
            sw.Stop();
            Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms");
            const int times = 1;

            IEnumerable<Task> RunOneAccount(string accountId)
            {
                Debug.Assert(client != null, nameof(client) + " != null");
                var account = client.GetGrain<IAccount>(accountId);
                foreach (var task in Enumerable.Range(0, times)
                    .Select(i => account.TransferIn(rd.Next(0, 100), Guid.NewGuid().ToString())))
                {
                    yield return task;
                }
            }
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