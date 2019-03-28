using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelloClaptrap.Interfaces.DomainService.TransferAccountBalance;
using Orleans;

namespace HelloClaptrap.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("start to connect");
            var clusterClient = await ConnectToCluster();
            Console.WriteLine("connect to cluster success");

            var random = new Random();
            var sw = Stopwatch.StartNew();
            for (var round = 0; round < int.MaxValue; round++)
            {
                Thread.Sleep(500);
                Console.WriteLine($"round {round} now");
                sw.Restart();
                const int count = 1000;
                var tasks = Enumerable.Range(round * count, count).Select(x =>
                {
                    var transferAccountBalance = clusterClient.GetGrain<ITransferAccountBalance>(x.ToString());
                    return transferAccountBalance.Transfer(GetRandomAccountId(), GetRandomAccountId(), 1);
                });
                await Task.WhenAll(tasks);

                Console.WriteLine($"finished in {sw.ElapsedMilliseconds} ms");
            }
            
            string GetRandomAccountId()
            {
                return random.Next(0, 500).ToString();
            }
        }

        private static async Task<IClusterClient> ConnectToCluster()
        {
            var clientBuilder = new ClientBuilder();
            clientBuilder.UseLocalhostClustering();
            var client = clientBuilder.Build();
            await client.Connect(exception => Task.FromResult(true));
            return client;
        }
    }
}