using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public class StartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var daprOptions = builder.ApplicationServices
                    .GetRequiredService<IOptions<ClaptrapServerOptions>>()
                    .Value.Dapr;
                next.Invoke(builder);
                builder.UseEndpoints(endpoints =>
                {
                    endpoints.MapPost(daprOptions.Topic, RequestDelegate)
                        .WithTopic(daprOptions.PubsubName, daprOptions.Topic);
                });
            };
        }

        private static async Task RequestDelegate(HttpContext context)
        {
            using var serviceScope = context.RequestServices.CreateScope();
            var handler = serviceScope.ServiceProvider.GetRequiredService<IClaptrapHandler>();
            await handler.HandleAsync(context);
        }
    }
}