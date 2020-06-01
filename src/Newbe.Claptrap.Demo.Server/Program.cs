using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.Bootstrapper;
using Newtonsoft.Json;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        public static Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ExceptionObject);
            };
            ClaptrapMetrics.MetricsRoot = new MetricsBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();
            var metrics = ClaptrapMetrics.MetricsRoot;
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(context =>
                {
                    var serviceProviderFactory = new AutofacServiceProviderFactory(
                        builder =>
                        {
                            var collection = new ServiceCollection().AddLogging(logging =>
                            {
                                logging.AddConsole();
                                logging.SetMinimumLevel(LogLevel.Debug);
                            });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, builder);
                            const string mysqlConnectionString =
                                "Server=localhost;Database=claptrap;Uid=root;Pwd=claptrap;Pooling=True;";
                            const string postgreSQLConnectionString =
                                "Server=localhost;Port=5432;Database=claptrap;User Id=postgres;Password=claptrap;CommandTimeout=20;Timeout=15;Pooling=true;MinPoolSize=1;MaxPoolSize=20;";
                            const string mongoConnectionString =
                                "mongodb://root:claptrap@localhost/claptrap?authSource=admin";
                            var claptrapBootstrapper = bootstrapperBuilder
                                .ScanClaptrapModule()
                                .ScanClaptrapDesigns(new[]
                                {
                                    typeof(AccountGrain).Assembly
                                })
                                .UseSQLiteAsTestingStorage()
                                // .AddConnectionString("claptrap",
                                //     mysqlConnectionString)
                                // .UseMySql(mysql =>
                                //     mysql
                                //         .AsEventStore(eventStore =>
                                //             eventStore.SharedTable())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.SharedTable())
                                // )
                                // .AddConnectionString("claptrap",
                                //     postgreSQLConnectionString)
                                // .UsePostgreSQL(postgreSQL =>
                                //     postgreSQL
                                //         .AsEventStore(eventStore =>
                                //             eventStore.SharedTable())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.SharedTable())
                                // )
                                // .AddConnectionString("claptrap",
                                //     mongoConnectionString)
                                // .UseMongoDB(mongoDb =>
                                //     mongoDb
                                //         .AsEventStore(eventStore =>
                                //             eventStore.SharedCollection())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.SharedCollection())
                                // )
                                .Build();
                            claptrapBootstrapper.Boot();
                            var store = claptrapBootstrapper.DumpDesignStore();
                            File.WriteAllText("design.json", JsonConvert.SerializeObject(store, Formatting.Indented));
                        });


                    return serviceProviderFactory;
                })
                .UseOrleans(siloBuilder =>
                {
                    siloBuilder
                        .UseLocalhostClustering()
                        .ConfigureLogging(logging =>
                        {
                            logging.SetMinimumLevel(LogLevel.Debug);
                            logging.AddFilter(
                                (s, level) => s.StartsWith("Microsoft.Orleans") && level >= LogLevel.Error);
                            logging.AddFilter((s, level) => s.Contains("Claptrap") && level >= LogLevel.Error);
                        })
                        .ConfigureApplicationParts(manager =>
                            manager.AddFromDependencyContext().WithReferences())
                        // .UseDashboard(options => options.Port = 9000)
                        ;
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureMetrics(metrics)
                .UseMetrics(
                    options =>
                    {
                        options.EndpointOptions = endpointsOptions =>
                        {
                            endpointsOptions.MetricsTextEndpointOutputFormatter = metrics
                                .OutputMetricsFormatters
                                .OfType<MetricsPrometheusTextOutputFormatter>()
                                .First();
                            endpointsOptions.MetricsEndpointOutputFormatter = metrics
                                .OutputMetricsFormatters
                                .OfType<MetricsPrometheusProtobufOutputFormatter>()
                                .First();
                        };
                    })
                .RunConsoleAsync();
        }
    }
}