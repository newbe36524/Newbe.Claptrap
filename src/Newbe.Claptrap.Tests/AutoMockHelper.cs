using System;
using Autofac;
using Autofac.Extras.Moq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.Localization.Modules;

namespace Newbe.Claptrap.Tests
{
    public static class AutoMockHelper
    {
        public static AutoMock Create(
            bool localizationModule = true,
            DateTime? nowTime = null,
            bool verifyAll = true,
            Action<ContainerBuilder> builderAction = null)
        {
            var action = builderAction ?? (builder => { });
            var mocker = AutoMock.GetStrict(builder =>
            {
                action(builder);
                builder.RegisterModule<TestLoggingModule>();

                if (localizationModule)
                {
                    var localizationOptions = new LocalizationOptions();
                    builder.Register(t => new OptionsWrapper<LocalizationOptions>(localizationOptions))
                        .As<IOptions<LocalizationOptions>>()
                        .SingleInstance();
                    builder.RegisterType<ResourceManagerStringLocalizerFactory>()
                        .As<IStringLocalizerFactory>()
                        .SingleInstance();
                    builder.RegisterGeneric(typeof(StringLocalizer<>))
                        .As(typeof(IStringLocalizer<>))
                        .InstancePerLifetimeScope();
                    builder.RegisterModule(new LocalizationModule(new ClaptrapLocalizationOptions()));
                }

                if (nowTime.HasValue)
                {
                    builder.RegisterInstance(new StaticClock(nowTime.Value))
                        .As<IClock>()
                        .SingleInstance();
                }
            });
            if (verifyAll)
            {
                mocker.VerifyAll = true;
            }

            return mocker;
        }
    }
}