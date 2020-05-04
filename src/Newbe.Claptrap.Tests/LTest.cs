using System.Globalization;
using Autofac;
using FluentAssertions;
using Newbe.Claptrap.Preview.Impl.Localization;
using Newbe.Claptrap.Preview.Impl.Modules;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class LTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public LTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Globalization()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new LocalizationModule());
            var container = builder.Build();

            var l = container.Resolve<IL>();
            var globalString = l[LK.L0001AutofacClaptrapBootstrapperBuilder.L001BuildException];
            globalString.Should().NotBeNullOrEmpty();
            globalString.Should().Be("failed to build claptrap bootstrapper");
            _testOutputHelper.WriteLine(globalString);
        }

        [Fact]
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
            _testOutputHelper.WriteLine(localString);
        }

        [Fact]
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
            _testOutputHelper.WriteLine(localString);
        }
    }
}