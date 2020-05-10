using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using FluentAssertions;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Design;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class GlobalClaptrapDesignStoreConfiguratorTest
    {
        [Theory]
        [TestCase(nameof(ClaptrapDesign.EventLoaderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.EventSaverFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateLoaderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateSaverFactoryType))]
        [TestCase(nameof(ClaptrapDesign.InitialStateDataFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateHolderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.EventHandlerFactoryFactoryType))]
        public void NullFactoryType(string propertyName)
        {
            var globalDesignFactoryType = typeof(int);
            using var autoMock = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    var globalClaptrapDesign = new GlobalClaptrapDesign();
                    var propertyInfo = typeof(GlobalClaptrapDesign).GetProperty(propertyName);
                    Debug.Assert(propertyInfo != null, nameof(propertyInfo) + " != null");
                    propertyInfo.SetValue(globalClaptrapDesign, globalDesignFactoryType);
                    builder.RegisterInstance(globalClaptrapDesign)
                        .AsImplementedInterfaces();
                });

            var configurator = autoMock.Create<GlobalClaptrapDesignStoreConfigurator>();
            var claptrapDesignStore = new ClaptrapDesignStore();
            var claptrapDesign = new ClaptrapDesign
            {
                Identity = TestClaptrapIdentity.Instance
            };
            var property = typeof(ClaptrapDesign).GetProperty(propertyName);
            Debug.Assert(property != null, nameof(property) + " != null");
            property.SetValue(claptrapDesign, null);
            claptrapDesignStore.AddOrReplace(claptrapDesign);
            configurator.Configure(claptrapDesignStore);

            var design = claptrapDesignStore.Single();
            var value = property.GetValue(design);
            value.Should().Be(globalDesignFactoryType);
        }

        [Theory]
        [TestCase(nameof(ClaptrapDesign.EventLoaderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.EventSaverFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateLoaderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateSaverFactoryType))]
        [TestCase(nameof(ClaptrapDesign.InitialStateDataFactoryType))]
        [TestCase(nameof(ClaptrapDesign.StateHolderFactoryType))]
        [TestCase(nameof(ClaptrapDesign.EventHandlerFactoryFactoryType))]
        public void HaveFactoryType(string propertyName)
        {
            var globalDesignFactoryType = typeof(int);
            using var autoMock = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    var globalClaptrapDesign = new GlobalClaptrapDesign();
                    var propertyInfo = typeof(GlobalClaptrapDesign).GetProperty(propertyName);
                    Debug.Assert(propertyInfo != null, nameof(propertyInfo) + " != null");
                    propertyInfo.SetValue(globalClaptrapDesign, globalDesignFactoryType);
                    builder.RegisterInstance(globalClaptrapDesign)
                        .AsImplementedInterfaces();
                });

            var configurator = autoMock.Create<GlobalClaptrapDesignStoreConfigurator>();
            var claptrapDesignStore = new ClaptrapDesignStore();
            var claptrapDesign = new ClaptrapDesign
            {
                Identity = TestClaptrapIdentity.Instance
            };
            var property = typeof(ClaptrapDesign).GetProperty(propertyName);
            Debug.Assert(property != null, nameof(property) + " != null");
            var oldDesignFactoryType = typeof(int);
            property.SetValue(claptrapDesign, oldDesignFactoryType);
            claptrapDesignStore.AddOrReplace(claptrapDesign);
            configurator.Configure(claptrapDesignStore);

            var design = claptrapDesignStore.Single();
            var value = property.GetValue(design);
            value.Should().Be(oldDesignFactoryType);
        }

        [Theory]
        [TestCase(nameof(ClaptrapOptions.StateSavingOptions))]
        [TestCase(nameof(ClaptrapOptions.StateRecoveryOptions))]
        [TestCase(nameof(ClaptrapOptions.EventLoadingOptions))]
        public void NullClaptrapOptions(string propertyName)
        {
            var propertyInfo = typeof(ClaptrapOptions).GetProperty(propertyName);
            Debug.Assert(propertyInfo != null, nameof(propertyInfo) + " != null");
            var globalOption = Activator.CreateInstance(propertyInfo.PropertyType);
            using var autoMock = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    var claptrapOptions = new ClaptrapOptions();
                    var globalClaptrapDesign = new GlobalClaptrapDesign
                    {
                        ClaptrapOptions = claptrapOptions
                    };
                    propertyInfo.SetValue(claptrapOptions, globalOption);
                    builder.RegisterInstance(globalClaptrapDesign)
                        .AsImplementedInterfaces();
                });

            var configurator = autoMock.Create<GlobalClaptrapDesignStoreConfigurator>();
            var claptrapDesignStore = new ClaptrapDesignStore();
            var claptrapDesign = new ClaptrapDesign
            {
                Identity = TestClaptrapIdentity.Instance,
                ClaptrapOptions = new ClaptrapOptions()
            };
            propertyInfo.SetValue(claptrapDesign.ClaptrapOptions, null);
            claptrapDesignStore.AddOrReplace(claptrapDesign);
            configurator.Configure(claptrapDesignStore);

            var design = claptrapDesignStore.Single();
            var value = propertyInfo.GetValue(design.ClaptrapOptions);
            value.Should().Be(globalOption);
        }

        [Theory]
        [TestCase(nameof(ClaptrapOptions.StateSavingOptions))]
        [TestCase(nameof(ClaptrapOptions.StateRecoveryOptions))]
        [TestCase(nameof(ClaptrapOptions.EventLoadingOptions))]
        public void HaveClaptrapOptions(string propertyName)
        {
            var propertyInfo = typeof(ClaptrapOptions).GetProperty(propertyName);
            Debug.Assert(propertyInfo != null, nameof(propertyInfo) + " != null");
            var globalOption = Activator.CreateInstance(propertyInfo.PropertyType);
            using var autoMock = AutoMockHelper.Create(
                builderAction: builder =>
                {
                    var claptrapOptions = new ClaptrapOptions();
                    var globalClaptrapDesign = new GlobalClaptrapDesign
                    {
                        ClaptrapOptions = claptrapOptions
                    };
                    propertyInfo.SetValue(claptrapOptions, globalOption);
                    builder.RegisterInstance(globalClaptrapDesign)
                        .AsImplementedInterfaces();
                });

            var configurator = autoMock.Create<GlobalClaptrapDesignStoreConfigurator>();
            var claptrapDesignStore = new ClaptrapDesignStore();
            var claptrapDesign = new ClaptrapDesign
            {
                Identity = TestClaptrapIdentity.Instance,
                ClaptrapOptions = new ClaptrapOptions()
            };
            var designOption = Activator.CreateInstance(propertyInfo.PropertyType);
            propertyInfo.SetValue(claptrapDesign.ClaptrapOptions, designOption);
            claptrapDesignStore.AddOrReplace(claptrapDesign);
            configurator.Configure(claptrapDesignStore);

            var design = claptrapDesignStore.Single();
            var value = propertyInfo.GetValue(design.ClaptrapOptions);
            value.Should().Be(designOption);
            designOption.Should().NotBe(globalOption);
        }
    }
}