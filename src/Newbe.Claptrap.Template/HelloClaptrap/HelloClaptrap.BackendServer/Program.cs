using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloClaptrap.Actors.AuctionItem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Bootstrapper;

namespace HelloClaptrap.BackendServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseClaptrap(builder => { builder.ScanClaptrapDesigns(new[] {typeof(AuctionItemActor).Assembly}); },
                    builder => { })
                .ConfigureServices((context, collection) => { })
                .UseClaptrapHostCommon()
                .UseClaptrapDaprHost()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}