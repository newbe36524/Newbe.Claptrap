using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.CapacityBurning
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                // logging.AddConsole();
            });
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            containerBuilder.RegisterType<Burning>()
                .AsSelf()
                .InstancePerDependency();
            var builder = new AutofacClaptrapBootstrapperBuilder(new NullLoggerFactory(), containerBuilder);
            var claptrapBootstrapper = builder
                .ScanClaptrapDesigns(new[]
                {
                    typeof(Burning),
                    typeof(IBurning),
                })
                .ScanClaptrapModule()
                .UseSQLiteAsTestingStorage()
                .Build();
            claptrapBootstrapper.Boot();

            var container = containerBuilder.Build();
            var factory = container.Resolve<Burning.Factory>();
            var sw = Stopwatch.StartNew();
            const int maxBurningCount = 100;
            var burnings = Enumerable.Range(0, maxBurningCount)
                .Select(x => factory.Invoke(new ClaptrapIdentity(x.ToString(), Codes.Burning)))
                .ToArray();
            await Task.WhenAll(burnings.Select(x => x.ActivateAsync()));
            Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms to activate all");
            
            var total = Stopwatch.StartNew();
            var rd = new Random();
            for (var i = 0; i < 1_000_000; i++)
            {
                sw.Restart();
                var tasks = Enumerable.Range(0, 1_000)
                    .Select(x =>
                    {
                        var id = rd.Next(0, maxBurningCount);
                        return burnings[id].HandleOneAsync();
                    })
                    .ToArray();
                await Task.WhenAll(tasks);
                Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms, {i}");
            }
            
            Console.WriteLine($"total cost {total.ElapsedMilliseconds} ms");

            // var container = containerBuilder.Build();
            // var factory = (ClaptrapFactory) container.Resolve<IClaptrapFactory>();
            // var id = new ClaptrapIdentity("1", Codes.Burning);
            // var lifetimeScope = factory.BuildClaptrapLifetimeScope(id);
            // var eventSaver = lifetimeScope.Resolve<IEventSaver>();
            // var sw = Stopwatch.StartNew();
            //
            // var total = Stopwatch.StartNew();
            // for (var i = 0; i < 1_000_000; i++)
            // {
            //     sw.Restart();
            //     const int batchItemCount = 1_000;
            //     var tasks = Enumerable.Range(i * batchItemCount, batchItemCount)
            //         .Select(x =>
            //         {
            //             var unitEvent = new UnitEvent(id, Codes.BurningEvent,
            //                 UnitEvent.UnitEventData.Create())
            //             {
            //                 Version = x,
            //             };
            //             return eventSaver.SaveEventAsync(unitEvent);
            //         });
            //     await Task.WhenAll(tasks);
            //     Thread.Sleep(1);
            //     Console.WriteLine($"cost {sw.ElapsedMilliseconds} ms, {(i + 1) * batchItemCount}");
            // }
            //
            // Console.WriteLine($"total cost {total.ElapsedMilliseconds} ms");
        }
    }
}