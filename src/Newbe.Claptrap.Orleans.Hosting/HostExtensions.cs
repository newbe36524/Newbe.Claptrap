using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Formatters.Prometheus;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap;
using Newbe.Claptrap.Bootstrapper;
using Orleans;
using Orleans.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseClaptrap(this IHostBuilder hostBuilder,
            params Assembly[] assemblies)
            =>
                hostBuilder.UseClaptrap(builder => builder.ScanClaptrapDesigns(assemblies));

        public static IHostBuilder UseClaptrap(this IHostBuilder hostBuilder,
            Action<IClaptrapBootstrapperBuilder> builderAction,
            Action<ContainerBuilder>? containerBuilderAction = null)
        {
            return hostBuilder
                .ConfigureServices((context, collection) =>
                {
                    var configSection =
                        context.Configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
                    collection.Configure<ClaptrapServerOptions>(configSection);
                })
                .UseServiceProviderFactory(context =>
                {
                    var serviceProviderFactory = new AutofacServiceProviderFactory(
                        builder =>
                        {
                            containerBuilderAction?.Invoke(builder);
                            var collection = new ServiceCollection().AddLogging(logging =>
                            {
                                logging.SetMinimumLevel(LogLevel.Debug);
                            });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, builder);
                            bootstrapperBuilder
                                .ScanClaptrapModule()
                                .AddConfiguration(context);
                            builderAction.Invoke(bootstrapperBuilder);
                            var claptrapBootstrapper = bootstrapperBuilder.Build();
                            claptrapBootstrapper.Boot();
                        });

                    return serviceProviderFactory;
                });
        }

        public static IHostBuilder UseOrleansClaptrap(this IHostBuilder hostBuilder)
            => hostBuilder
                .UseOrleans((context, builder) =>
                {
                    var claptrapOptions = new ClaptrapServerOptions();
                    var config =
                        context.Configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
                    config.Bind(claptrapOptions);
                    var claptrapOptionsOrleans = claptrapOptions.Orleans;
                    var hostname = claptrapOptionsOrleans.Hostname ?? "localhost";
                    if (!IPAddress.TryParse(hostname, out var ip))
                    {
                        ip = Dns.GetHostEntry(hostname).AddressList.First();
                    }

                    const int defaultGatewayPort = 30000;
                    const int defaultSiloPort = 11111;
                    var gatewayPort = claptrapOptionsOrleans.GatewayPort
                                      ?? defaultGatewayPort;
                    var siloPort = claptrapOptionsOrleans.SiloPort
                                   ?? defaultSiloPort;
                    builder
                        .ConfigureDefaults()
                        .UseLocalhostClustering()
                        .ConfigureEndpoints(ip, siloPort, gatewayPort)
                        .ConfigureApplicationParts(manager =>
                            manager.AddFromDependencyContext().WithReferences());
                })
                .ConfigureMetricsWithDefaults((context, builder) =>
                {
                    var claptrapOptions = new ClaptrapServerOptions();
                    var config =
                        context.Configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
                    config.Bind(claptrapOptions);
                    var metricsInfluxDbOptions = claptrapOptions.MetricsInfluxDb;
                    if (metricsInfluxDbOptions != null)
                    {
                        if (metricsInfluxDbOptions.Enabled == true)
                        {
                            builder.Report
                                .ToInfluxDb(options =>
                                {
                                    options.InfluxDb.Database = metricsInfluxDbOptions?.Database
                                                                ?? "metricsdatabase";
                                    options.InfluxDb.UserName = metricsInfluxDbOptions?.UserName
                                                                ?? "claptrap";
                                    options.InfluxDb.Password = metricsInfluxDbOptions?.Password
                                                                ?? "claptrap";
                                    options.InfluxDb.BaseUri = metricsInfluxDbOptions?.BaseUri
                                                               ?? new Uri("http://127.0.0.1:19086");
                                    options.InfluxDb.CreateDataBaseIfNotExists =
                                        metricsInfluxDbOptions?.CreateDataBaseIfNotExists
                                        ?? true;
                                    options.HttpPolicy.BackoffPeriod = metricsInfluxDbOptions?.BackoffPeriod
                                                                       ?? TimeSpan.FromSeconds(30);
                                    options.HttpPolicy.FailuresBeforeBackoff =
                                        metricsInfluxDbOptions?.FailuresBeforeBackoff
                                        ?? 5;
                                    options.HttpPolicy.Timeout = metricsInfluxDbOptions?.Timeout
                                                                 ?? TimeSpan.FromSeconds(10);
                                    options.FlushInterval = metricsInfluxDbOptions?.FlushInterval
                                                            ?? TimeSpan.FromSeconds(20);
                                    options.MetricsOutputFormatter =
                                        new MetricsInfluxDbLineProtocolOutputFormatter();
                                });
                        }
                    }
                })
                .UseMetrics(options =>
                {
                    options.EndpointOptions = endpointsOptions =>
                    {
                        endpointsOptions.MetricsTextEndpointOutputFormatter =
                            new MetricsPrometheusTextOutputFormatter();
                    };
                });
    }
}