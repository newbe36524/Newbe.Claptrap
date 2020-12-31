using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
using NUnit.Framework;

namespace Newbe.Claptrap.Dapr.Hosting.Tests
{
    public class HostTest
    {
        [TestCase("sqlite")]
        [TestCase("mongodb")]
        [TestCase("mysql")]
        [TestCase("postgresql")]
        [TestCase("multiple_db")]
        public async Task BuildHost(string jsonFileName)
        {
            var hostBuilder = new HostBuilder();
            var host = hostBuilder
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile($"configs/load_db_config/{jsonFileName}.json");
                })
                .UseClaptrap(builder =>
                {
                    builder.ScanClaptrapDesigns(new[]
                    {
                        typeof(IAccount),
                        typeof(Account),
                        typeof(IAccountBalanceMinion),
                        typeof(AccountBalanceMinion)
                    });
                })
                .UseClaptrapDaprHost()
                .Build();
            await host.StartAsync();
            await host.StopAsync();
        }
    }
}