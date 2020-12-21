using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.CapacityBurning.Grains;
using Newbe.Claptrap.CapacityBurning.Module;
using Newbe.Claptrap.CapacityBurning.Services;
using NLog.Web;

namespace Newbe.Claptrap.CapacityBurning
{
    internal class Program
    {
        private static void Main(string[] args)
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .UseClaptrapOrleansHost()
                .ConfigureAppConfiguration(builder =>
                {
                    var configBuilder = new ConfigurationBuilder();
                    configBuilder.AddJsonFile("appsettings.json");
                    var config = configBuilder.Build();
                    var options = new BurningDatabaseOptions();
                    config.GetSection("BurningDatabase").Bind(options);
                    builder.AddJsonFile($"Claptrap/claptrap.{options.DatabaseType:G}.json");
                })
                .ConfigureServices((context, collection) =>
                {
                    collection.Configure<BurningDatabaseOptions>(options =>
                    {
                        context.Configuration.GetSection("BurningDatabase").Bind(options);
                    });
                })
                .UseServiceProviderFactory(context =>
                {
                    var serviceProviderFactory = new AutofacServiceProviderFactory(
                        builder =>
                        {
                            builder.RegisterModule<BurningModule>();

                            var collection = new ServiceCollection()
                                .AddLogging(logging => { logging.SetMinimumLevel(LogLevel.Debug); });
                            var buildServiceProvider = collection.BuildServiceProvider();
                            var loggerFactory = buildServiceProvider.GetRequiredService<ILoggerFactory>();
                            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory);
                            var claptrapBootstrapper = (AutofacClaptrapBootstrapper) bootstrapperBuilder
                                .ScanClaptrapModule()
                                .AddConfiguration(context)
                                .ScanClaptrapDesigns(new[]
                                {
                                    typeof(Burning).Assembly
                                })
                                .Build();
                            claptrapBootstrapper.Builder = builder;
                            claptrapBootstrapper.Boot();
                        });


                    return serviceProviderFactory;
                });
        }
    }
}