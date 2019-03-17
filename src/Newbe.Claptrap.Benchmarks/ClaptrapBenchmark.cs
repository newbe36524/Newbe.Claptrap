using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BenchmarkDotNet.Attributes;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo;
using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
using Newbe.Claptrap.EventChannels;
using Newbe.Claptrap.EventHub.DirectClient;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Benchmarks
{
    public class ClaptrapBenchmark
    {
        [Params(1, 10, 100, 1000, 10000)] 
        public int Times { get; set; }

        private IClusterClient _clusterClient;
        private ISiloHost _siloHost;
        private readonly byte[] _randomBytes = new byte[20000];
        private ConcurrentDictionary<string, decimal> _balanceDic;
        private const int DefaultBalance = 10000000;
        
        [IterationSetup]
        public void Setup()
        {
            _balanceDic = new ConcurrentDictionary<string, decimal>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                _balanceDic.AddOrUpdate(i.ToString(), DefaultBalance,(s, arg2) => DefaultBalance);
            }
        }

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            new Random(36524).NextBytes(_randomBytes);
            _balanceDic = new ConcurrentDictionary<string, decimal>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                _balanceDic.TryAdd(i.ToString(), DefaultBalance);
            }

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

        [Benchmark(Baseline = true, Description = "ConcurrentDictionary")]
        public async Task ConcurrentDictionarySimulation()
        {
            var tasks = Enumerable.Range(1, Times).Select(x =>
            {
                var index = x * 2;
                var fromId = _randomBytes[index].ToString();
                var toId = _randomBytes[index + 1].ToString();
                _balanceDic.AddOrUpdate(fromId, 1, (id, balance) => balance - 1);
                _balanceDic.AddOrUpdate(toId, 1, (id, balance) => balance + 1);
                return Task.CompletedTask;
            });
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        [Benchmark(Description = "Claptrap")]
        public async Task ClaptrapMethod()
        {
            var tasks = Enumerable.Range(1, Times).Select(x =>
            {
                var index = x * 2;
                var fromId = _randomBytes[index].ToString();
                var toId = _randomBytes[index + 1].ToString();
                var transferAccountBalance = _clusterClient.GetGrain<ITransferAccountBalance>(x.ToString());
                return transferAccountBalance.Transfer(fromId, toId, 1);
            });
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}