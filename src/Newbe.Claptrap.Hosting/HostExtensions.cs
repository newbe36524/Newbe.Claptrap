using System;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IServiceCollection AddClaptrapServerOptions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddOptions<ClaptrapServerOptions>()
                .BindConfiguration(ClaptrapServerOptions.ConfigurationSectionName);
            return serviceCollection;
        }

        public static IHostBuilder UseClaptrapMetrics(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureMetricsWithDefaults((context, builder) =>
                {
                    var claptrapOptions = new ClaptrapServerOptions();
                    var config =
                        context.Configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
                    config.Bind(claptrapOptions);
                    var metricsInfluxDbOptions = claptrapOptions.MetricsInfluxDb;
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