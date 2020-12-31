using System;
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
using Newbe.Claptrap.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseClaptrap(this IHostBuilder hostBuilder,
            params Assembly[] assemblies)
        {
            return hostBuilder.UseClaptrap(builder => builder.ScanClaptrapDesigns(assemblies));
        }

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
                            var collection = new ServiceCollection().AddLogging(logging =>
                            {
                                logging.SetMinimumLevel(LogLevel.Debug);
                            });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetRequiredService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory);
                            bootstrapperBuilder
                                .ScanClaptrapModule()
                                .AddConfiguration(context);
                            builderAction.Invoke(bootstrapperBuilder);
                            var claptrapBootstrapper = (AutofacClaptrapBootstrapper) bootstrapperBuilder.Build();
                            context.Properties[ClaptrapHostConst.HostBuilderContextClaptrapDesignStoreKey] =
                                claptrapBootstrapper.ClaptrapDesignStore;
                            claptrapBootstrapper.Builder = builder;
                            claptrapBootstrapper.Boot();
                            containerBuilderAction?.Invoke(builder);
                        });

                    return serviceProviderFactory;
                });
        }

        public static IHostBuilder UseClaptrapHostCommon(this IHostBuilder hostBuilder)
        {
            return hostBuilder
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
}