using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.DesignStoreFormatter;
using NLog.Web;
using Orleans;

namespace Newbe.Claptrap.Demo.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseOrleansClaptrap()
                .UseOrleans(builder => { builder.UseDashboard(options => options.Port = 9000); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
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
                            var loggerFactory = buildServiceProvider.GetService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory, builder);
                            var claptrapBootstrapper = bootstrapperBuilder
                                .ScanClaptrapModule()
                                .AddDefaultConfiguration(context)
                                .ScanClaptrapDesigns(new[]
                                {
                                    typeof(AccountGrain).Assembly
                                })
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
                });
    }
}