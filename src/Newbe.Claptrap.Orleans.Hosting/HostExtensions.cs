using System;
using System.Linq;
using System.Net;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.InfluxDB;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newbe.Claptrap.Orleans;
using Orleans;
using Orleans.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseOrleansClaptrap(this IHostBuilder hostBuilder)
            => hostBuilder
                .UseOrleans((context, builder) =>
                {
                    var claptrapOptions = new ClaptrapServeringOptions();
                    var config =
                        context.Configuration.GetSection(ClaptrapServeringOptions.ConfigurationSectionName);
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
                    var claptrapOptions = new ClaptrapServeringOptions();
                    var config =
                        context.Configuration.GetSection(ClaptrapServeringOptions.ConfigurationSectionName);
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