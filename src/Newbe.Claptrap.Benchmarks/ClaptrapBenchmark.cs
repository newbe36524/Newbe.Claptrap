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
        private const int RandomIdCount = 10000;

        [Params(100_000, 10_000, 1000, 100, 10, 1)]
        public int Times { get; set; }

        private IClusterClient _clusterClient;
        private ISiloHost _siloHost;
        private readonly string[] _randomIds = new string[RandomIdCount];
        private ConcurrentDictionary<string, decimal> _balanceDic;
        private const int DefaultBalance = 10000000;
        private int _transferAccountBalanceIdPrefix;

        [IterationSetup]
        public void Setup()
        {
            _balanceDic = new ConcurrentDictionary<string, decimal>();
            for (var i = 0; i < byte.MaxValue; i++)
            {
                _balanceDic.AddOrUpdate(i.ToString(), DefaultBalance, (s, arg2) => DefaultBalance);
            }

            _transferAccountBalanceIdPrefix++;
        }

        [GlobalSetup]
        public async Task GlobalSetup()
        {
            var bytes = new byte[RandomIdCount];
            new Random(36524).NextBytes(bytes);
            for (var i = 0; i < bytes.Length; i++)
            {
                _randomIds[i] = bytes[i].ToString();
            }

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
                    containerBuilder.RegisterModule<DirectClientEventHubModule>();
                    
                    var container = containerBuilder.Build();
                    var serviceProvider = new AutofacServiceProvider(container);
                    return serviceProvider;
                })
                .ConfigureApplicationParts(manager =>
                    manager.AddApplicationPart(typeof(DemoModule).Assembly).WithReferences());
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
            const int countPerRound = 1000;
            var round = Times / countPerRound + 1;
            for (var i = 0; i < round; i++)
            {
                var left = Times - i * countPerRound;
                var times = left > countPerRound ? countPerRound : left;
                if (times > 0)
                {
                    var tasks = Enumerable.Range(1, Times).Select(x =>
                    {
                        var index = x * 2;
                        var fromId = GetIdByIndex(index);
                        var toId = GetIdByIndex(index + 1);
                        _balanceDic.AddOrUpdate(fromId, 1, (id, balance) => balance - 1);
                        _balanceDic.AddOrUpdate(toId, 1, (id, balance) => balance + 1);
                        return Task.CompletedTask;
                    });
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
        }

        [Benchmark(Description = "Claptrap")]
        public async Task ClaptrapMethod()
        {
            const int countPerRound = 1000;
            var round = Times / countPerRound + 1;
            for (var i = 0; i < round; i++)
            {
                var left = Times - i * countPerRound;
                var times = left > countPerRound ? countPerRound : left;
                if (times > 0)
                {
                    var tasks = Enumerable.Range(1, times).Select(x =>
                    {
                        var index = x * 2;
                        var fromId = GetIdByIndex(index);
                        var toId = GetIdByIndex(index + 1);
                        var transferAccountBalance =
                            _clusterClient.GetGrain<ITransferAccountBalance>($"{_transferAccountBalanceIdPrefix}{x}");
                        return transferAccountBalance.Transfer(fromId, toId, 1);
                    });
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
            }
        }

        private string GetIdByIndex(int index)
        {
            return _randomIds[index % RandomIdCount];
        }
    }
}