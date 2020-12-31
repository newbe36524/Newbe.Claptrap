using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.DevTools
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"current dir : {AppDomain.CurrentDomain.BaseDirectory}");
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddLogging(logging => { logging.AddConsole(); });

                var builder = new ContainerBuilder();
                builder.Populate(serviceCollection);
                builder.RegisterModule<ToolsServiceModule>();
                var container = builder.Build();

                var toolService = container.Resolve<IToolService>();
                await toolService.RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}