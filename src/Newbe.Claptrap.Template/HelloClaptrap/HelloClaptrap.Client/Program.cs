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

            // TODO please add your code here and remove the exception
            throw new NotImplementedException();
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