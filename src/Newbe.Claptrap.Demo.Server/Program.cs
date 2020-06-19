using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Formatters.Prometheus;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.DesignStoreFormatter;
using Newbe.Claptrap.StorageProvider.Relational;
using Orleans;
using Orleans.Hosting;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        private static Timer? _timer;
        public static Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ExceptionObject);
            };
            ClaptrapMetrics.MetricsRoot = new MetricsBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Report
                .ToInfluxDb(options =>
                {
                    options.InfluxDb.Database = "metricsdatabase";
                    options.InfluxDb.CreateDataBaseIfNotExists = true;
                    options.InfluxDb.UserName = "claptrap";
                    options.InfluxDb.Password = "claptrap";
                    options.InfluxDb.BaseUri = new Uri("http://127.0.0.1:19086");
                    options.InfluxDb.CreateDataBaseIfNotExists = true;
                    options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
                    options.HttpPolicy.FailuresBeforeBackoff = 5;
                    options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
                    options.MetricsOutputFormatter = new MetricsInfluxDbLineProtocolOutputFormatter();
                    options.FlushInterval = TimeSpan.FromSeconds(20);
                })
                .Build();
     
            _timer= new Timer(1000);
            _timer.Elapsed += (sender, eventArgs) =>
            {
                Task.WhenAll(ClaptrapMetrics.MetricsRoot.ReportRunner.RunAllAsync()).Wait();
            };
            _timer.Start();
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
                                // .AddConnectionString(Defaults.ConnectionName,
                                //     mysqlConnectionString)
                                // .UseMySql(mysql =>
                                //     mysql
                                //         .AsEventStore(eventStore =>
                                //             eventStore.SharedTable())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.SharedTable())
                                // )
                                // .AddConnectionString(Defaults.ConnectionName,
                                //     postgreSQLConnectionString)
                                // .UsePostgreSQL(postgreSQL =>
                                //     postgreSQL
                                //         .AsEventStore(eventStore =>
                                //             eventStore.SharedTable())
                                //         .AsStateStore(stateStore =>
                                //             stateStore.SharedTable())
                                // )
                                // .AddConnectionString(Defaults.ConnectionName,
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
                            var json = claptrapBootstrapper.DumpDesignAsJson();
                            File.WriteAllText("design.json", json);
                            var markdown = claptrapBootstrapper.DumpDesignAsMarkdown(
                                new DesignStoreMarkdownFormatterOptions
                                {
                                    TrimSuffix = ClaptrapCodes.ApplicationDomain
                                });
                            File.WriteAllText("design.md", markdown);
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