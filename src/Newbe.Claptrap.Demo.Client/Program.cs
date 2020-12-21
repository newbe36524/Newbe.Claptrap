using System.Threading.Tasks;

namespace Newbe.Claptrap.Demo.Client
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // var clientBuilder = new ClientBuilder();
            // clientBuilder
            //     .UseLocalhostClustering()
            //     .UseServiceProviderFactory(collection =>
            //     {
            //         var containerBuilder = new ContainerBuilder();
            //
            //         // Once you've registered everything in the ServiceCollection, call
            //         // Populate to bring those registrations into Autofac. This is
            //         // just like a foreach over the list of things in the collection
            //         // to add them to Autofac.
            //         containerBuilder.Populate(collection);
            //
            //         // Creating a new AutofacServiceProvider makes the container
            //         // available to your app using the Microsoft IServiceProvider
            //         // interface so you can use those abstractions rather than
            //         // binding directly to Autofac.
            //         var container = containerBuilder.Build();
            //         var serviceProvider = new AutofacServiceProvider(container);
            //         return serviceProvider;
            //     });
            // var client = clientBuilder.Build();
            // Console.WriteLine("start to connect");
            // await client.Connect(exception => Task.FromResult(true));
            // Console.WriteLine("connected");
            // var rd = new Random();
            //
            // // var testSettings = (maxId : 1,maxTimes: 1);
            // // var testSettings = (maxId : 10,maxTimes: 100);
            // var testSettings = (maxId: 100, maxTimes: 100000);
            // var ids = Enumerable.Range(1, testSettings.maxId);
            // var sw = Stopwatch.StartNew();
            // var accounts = ids.Select(x => client.GetGrain<IAccount>(x.ToString())).ToArray();
            // var pageSize = 50;
            // var pageCount = testSettings.maxTimes / pageSize;
            // for (var i = 0; i < pageCount; i++)
            // {
            //     var tasks = Enumerable.Range(0, pageSize)
            //         .SelectMany(i => accounts.Select(a => a.TransferIn(rd.Next(0, 100))));
            //     await Task.WhenAll(tasks);
            // }
            //
            // sw.Stop();
            // Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms");

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
            // client.Dispose();
        }
    }
}