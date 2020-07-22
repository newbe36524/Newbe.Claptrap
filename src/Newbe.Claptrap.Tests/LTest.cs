using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap.Localization;
using Newbe.Claptrap.Localization.Modules;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class LTest
    {
        [Test]
        [SetUICulture("en-US")]
        public void UnsupportedCulture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLocalization();
            serviceCollection.AddLogging();
            var builder = new ContainerBuilder();
            builder.Populate(serviceCollection);
            builder.RegisterModule(new LocalizationModule(new ClaptrapLocalizationOptions()));
            var container = builder.Build();
            var l = container.Resolve<IL>();
            var result = l[LK.failed_to_build_claptrap_bootstrapper];
            result.Should().Be("failed to build claptrap bootstrapper");
        }

        [Test]
        [SetUICulture("zh-Hans")]
        public void SupportedCulture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLocalization();
            serviceCollection.AddLogging();
            var builder = new ContainerBuilder();
            builder.Populate(serviceCollection);
            builder.RegisterModule(new LocalizationModule(new ClaptrapLocalizationOptions()));
            var container = builder.Build();
            var l = container.Resolve<IL>();
            var result = l[LK.failed_to_build_claptrap_bootstrapper];
            result.Should().Be("无法构建 claptrap 启动器");
        }

        [Test]
        [SetUICulture("zh-Hans")]
        public void ReverseRegistration()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule(new ClaptrapLocalizationOptions()));
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLocalization();
            serviceCollection.AddLogging();
            builder.Populate(serviceCollection);
            var container = builder.Build();
            var l = container.Resolve<IL>();
            var result = l[LK.failed_to_build_claptrap_bootstrapper];
            result.Should().Be("无法构建 claptrap 启动器","it does not matter that registering LocalizationModule before AddLocalization or not");
        }
    }
}