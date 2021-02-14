using App.Metrics;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.StorageTestWebApi.Services;
using Newbe.Claptrap.TestSuit.QuickSetupTools;

namespace Newbe.Claptrap.StorageTestWebApi
{
    public class Startup
    {
        private readonly AutofacClaptrapBootstrapper _claptrapBootstrapper;
        private readonly IClaptrapDesignStore _claptrapDesignStore;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var loggerFactory = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .BuildServiceProvider()
                .GetRequiredService<ILoggerFactory>();

            var bootstrapperBuilder = new AutofacClaptrapBootstrapperBuilder(loggerFactory);
            _claptrapBootstrapper = (AutofacClaptrapBootstrapper) bootstrapperBuilder
                .ScanClaptrapModule()
                .AddConfiguration(configuration)
                .ScanClaptrapDesigns(new[]
                {
                    typeof(IAccount),
                    typeof(Account),
                    typeof(IAccountBalanceMinion),
                    typeof(AccountBalanceMinion),
                    typeof(IAccountHistoryBalanceMinion),
                    typeof(AccountHistoryBalanceMinion)
                })
                .ConfigureClaptrapDesign(x =>
                    x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.None)
                .Build();
            _claptrapDesignStore = _claptrapBootstrapper.DumpDesignStore();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddClaptrapServerOptions();
            services.AddOptions<TestConsoleOptions>()
                .BindConfiguration(nameof(TestConsoleOptions));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Newbe.Claptrap.StorageTestWebApi", Version = "v1"});
            });
        }
        
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register your own things directly with Autofac here. Don't
            // call builder.Populate(), that happens in AutofacServiceProviderFactory
            // for you.
            builder.RegisterType<Account>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterType<AccountBalanceMinion>()
                .AsSelf()
                .InstancePerDependency();
            builder.RegisterModule<StorageSetupModule>();
            builder.RegisterModule<StorageTestWebApiModule>();
            _claptrapBootstrapper.Boot(builder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var metricsRoot = app.ApplicationServices.GetRequiredService<IMetricsRoot>();
            ClaptrapMetrics.MetricsRoot = metricsRoot;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newbe.Claptrap.StorageTestWebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            using var serviceScope = app.ApplicationServices.CreateScope();
            var service = serviceScope.ServiceProvider.GetRequiredService<IInMemoryActorTestService>();
            service.InitAsync().Wait();
        }
    }
}