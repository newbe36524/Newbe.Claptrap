using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap.EventCenter.RabbitMQ;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests.RabbitMQ
{
    [Category("RabbitMQ"), Explicit]
    public class RabbitMQDatabaseTest : SQLiteSharedTableQuickSetupTest
    {
        protected override IEnumerable<string> AppsettingsFilenames
        {
            get { yield return "RabbitMQ/appsettings.json"; }
        }

        protected override async Task OnContainerBuilt(IServiceProvider container)
        {
            await base.OnContainerBuilt(container);
            var subscriberManager = container.GetRequiredService<IMQSubscriberManager>();
            await subscriberManager.StartAsync();
        }
    }
}