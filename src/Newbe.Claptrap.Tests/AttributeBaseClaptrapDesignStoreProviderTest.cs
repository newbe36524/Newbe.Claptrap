using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using Newbe.Claptrap.Design;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class AttributeBaseClaptrapDesignStoreProviderTest
    {
        private AttributeBaseClaptrapDesignStoreProvider.Factory GetFactory()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestLoggingModule());
            builder.RegisterType<ClaptrapDesignStore>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AttributeBaseClaptrapDesignStoreProvider>().AsSelf().InstancePerLifetimeScope();
            var container = builder.Build();

            var factory = container.Resolve<AttributeBaseClaptrapDesignStoreProvider.Factory>();
            return factory;
        }

        [Test]
        public void NothingToScan()
        {
            var factory = GetFactory();
            var provider = factory.Invoke(Enumerable.Empty<Type>());
            var claptrapDesignStore = provider.Create();
            claptrapDesignStore.Should().BeEmpty();
        }
    }
}