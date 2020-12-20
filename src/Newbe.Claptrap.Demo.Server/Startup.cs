using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newbe.Claptrap.AppMetrics;
using Newbe.Claptrap.Demo.Server.Services;
using Newbe.Claptrap.StorageSetup;
using Orleans;

namespace Newbe.Claptrap.Demo.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Newbe.Claptrap.Demo.Server", Version = "v1"});
            });
            services.AddOptions<TestConsoleOptions>()
                .Configure(
                    consoleOptions => Configuration.Bind(nameof(TestConsoleOptions), consoleOptions));

            services.AddHostedService<MuHost>();
            var clientBuilder = new ClientBuilder();
            clientBuilder
                .UseLocalhostClustering();
            var client = clientBuilder.Build();
            services.AddSingleton(client);
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
            builder.RegisterModule<StorageSetupModule>();
            builder.RegisterModule<DemoServerModule>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Newbe.Claptrap.StorageTestWebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });

            using var serviceScope = app.ApplicationServices.CreateScope();
            var service = serviceScope.ServiceProvider.GetRequiredService<IOrleansActorTestService>();
            service.InitAsync().Wait();

            Task.Run(() =>
            {
                using var scope = app.ApplicationServices.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IClusterClient>();
                client.Connect(e =>
                {
                    Console.WriteLine(e);
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    return Task.FromResult(true);
                });
            });

        }
    }
}