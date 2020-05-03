using System;
using System.Threading.Tasks;
using Autofac;

namespace Newbe.Claptrap.DevTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"current dir : {AppDomain.CurrentDomain.BaseDirectory}");
                var builder = new ContainerBuilder();
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