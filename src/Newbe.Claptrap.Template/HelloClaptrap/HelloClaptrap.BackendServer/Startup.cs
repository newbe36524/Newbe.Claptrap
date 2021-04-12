using System;
using Autofac;
using HelloClaptrap.Actors;
using HelloClaptrap.Actors.AuctionItem;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newbe.Claptrap;
using Newbe.Claptrap.Bootstrapper;
using OpenTelemetry.Trace;

namespace HelloClaptrap.BackendServer
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
                .ScanClaptrapDesigns(new[] {typeof(AuctionItemActor).Assembly})
                .UseDaprPubsub(pubsub => pubsub.AsEventCenter())
                .Build();
            _claptrapDesignStore = _claptrapBootstrapper.DumpDesignStore();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddClaptrapServerOptions();
            services.AddOpenTelemetryTracing(
                builder => builder
                    .AddSource(ClaptrapActivitySource.Instance.Name)
                    .SetSampler(new ParentBasedSampler(new AlwaysOnSampler()))
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddZipkinExporter(options =>
                    {
                        var zipkinBaseUri = Configuration.GetServiceUri("zipkin", "http");
                        options.Endpoint = new Uri(zipkinBaseUri!, "/api/v2/spans");
                    })
            );
            services.AddClaptrapServerOptions();
            services.AddActors(options => { options.AddClaptrapDesign(_claptrapDesignStore); });
            services.AddControllers()
                .AddDapr();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "HelloClaptrap.BackendServer", Version = "v1"});
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
            builder.RegisterModule<ActorsModule>();
            _claptrapBootstrapper.Boot(builder);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HelloClaptrap.BackendServer v1"));
            }


            app.UseRouting();
            
            app.UseCloudEvents();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapActorsHandlers();
                endpoints.MapSubscribeHandler();
                endpoints.MapControllers();
            });
        }
    }
}