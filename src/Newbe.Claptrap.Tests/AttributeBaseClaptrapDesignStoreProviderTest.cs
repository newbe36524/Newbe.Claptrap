using System;
using System.Linq;
using Autofac;
using FluentAssertions;
using Newbe.Claptrap.Design;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class AttributeBaseClaptrapDesignStoreProviderTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AttributeBaseClaptrapDesignStoreProviderTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private AttributeBaseClaptrapDesignStoreProvider.Factory GetFactory()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new XunitLoggingModule(_testOutputHelper));
            builder.RegisterType<ClaptrapDesignStore>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AttributeBaseClaptrapDesignStoreProvider>().AsSelf().InstancePerLifetimeScope();
            var container = builder.Build();

            var factory = container.Resolve<AttributeBaseClaptrapDesignStoreProvider.Factory>();
            return factory;
        }

        [Fact]
        public void NothingToScan()
        {
            var factory = GetFactory();
            var provider = factory.Invoke(Enumerable.Empty<Type>());
            var claptrapDesignStore = provider.Create();
            claptrapDesignStore.Should().BeEmpty();
        }
    }
}