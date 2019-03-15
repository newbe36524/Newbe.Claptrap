using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo;
using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.EventHub.DirectClient;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Benchmarks
{
    [CoreJob]
    [RankColumn, MinColumn, MaxColumn]
    public class ClaptrapBenchmark
    {
        [Params(1, 10, 100, 1000, 10000)] public int N;

        private IClusterClient _clusterClient;
        private ISiloHost _siloHost;

        [GlobalSetup]
        public async Task Setup()
        {
            var hostBuilder = new SiloHostBuilder();

            hostBuilder
                .UseLocalhostClustering()
                .UseServiceProviderFactory(collection =>
                {
                    var containerBuilder = new ContainerBuilder();

                    containerBuilder.Populate(collection);

                    containerBuilder.RegisterModule<DemoModule>();
                    containerBuilder.RegisterModule<ClaptrapServerModule>();

                    containerBuilder.RegisterType<DirectClientEventPublishChannelProvider>()
                        .As<IEventPublishChannelProvider>();
                    var container = containerBuilder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                })
                .ConfigureApplicationParts(manager =>
                    manager.AddApplicationPart(typeof(DemoModule).Assembly).WithReferences())
                .EnableDirectClient()
                ;
            _siloHost = hostBuilder.Build();
            await _siloHost.StartAsync();

            var clientBuilder = new ClientBuilder();
            clientBuilder
                .UseLocalhostClustering()
                .UseServiceProviderFactory(collection =>
                {
                    var containerBuilder = new ContainerBuilder();

                    containerBuilder.Populate(collection);
                    var container = containerBuilder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                });
            _clusterClient = clientBuilder.Build();
            await _clusterClient.Connect(exception => Task.FromResult(true));
        }

        [Benchmark]
        public async Task ClaptrapMethod()
        {
            var random = new Random();
            var tasks = Enumerable.Range(1, N).Select(x =>
            {
                var transferAccountBalance = _clusterClient.GetGrain<ITransferAccountBalance>(x.ToString());
                return transferAccountBalance.Transfer(GetRandomAccountId(), GetRandomAccountId(), 1);
            });
            await Task.WhenAll(tasks).ConfigureAwait(false);

            string GetRandomAccountId()
            {
                return random.Next(0, 500).ToString();
            }
        }
    }

    [CoreJob]
    [RankColumn, MinColumn, MaxColumn]
    public class TestBenchmark
    {
        [Params(1, 10, 100, 1000)] public int N { get; set; }

        [Benchmark]
        public async Task TaskDelay()
        {
            await Task.Delay(N * 10);
        }

        [Benchmark]
        public void ThreadSleep()
        {
            Thread.Sleep(N * 10);
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ClaptrapBenchmark>();
        }
    }
}