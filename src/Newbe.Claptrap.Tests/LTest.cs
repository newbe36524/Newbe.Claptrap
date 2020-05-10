using System;
using System.Globalization;
using Autofac;
using FluentAssertions;
using Newbe.Claptrap.Localization.Modules;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class LTest
    {
        [Test]
        public void Globalization()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule());
            var container = builder.Build();

            var l = container.Resolve<IL>();
            var globalString = l[LK.L0001AutofacClaptrapBootstrapperBuilder.L001BuildException];
            globalString.Should().NotBeNullOrEmpty();
            globalString.Should().Be("failed to build claptrap bootstrapper");
            Console.WriteLine(globalString);
        }

        [Test]
        public void Localization()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule());
            var container = builder.Build();

            CultureInfo.CurrentCulture = new CultureInfo("cn");
            var l = container.Resolve<IL>();
            var localString = l[LK.L0001AutofacClaptrapBootstrapperBuilder.L001BuildException];
            localString.Should().Be("构建 claptrap 启动器失败");
            localString.Should().NotBeNullOrEmpty();
            Console.WriteLine(localString);
        }

        [Test]
        public void LocalizationWithModuleSettings()
        {
            var builder = new ContainerBuilder();
            var culture = new CultureInfo("cn");
            builder.RegisterModule(new LocalizationModule(culture));
            var container = builder.Build();

            var l = container.Resolve<IL>();
            var localString = l[LK.L0001AutofacClaptrapBootstrapperBuilder.L001BuildException];
            localString.Should().Be("构建 claptrap 启动器失败");
            localString.Should().NotBeNullOrEmpty();
            Console.WriteLine(localString);
        }
    }
}