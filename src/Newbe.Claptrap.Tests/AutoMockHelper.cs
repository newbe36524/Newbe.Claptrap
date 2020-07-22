using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.Moq;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
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
                        .AsImplementedInterfaces()
                        .SingleInstance();
                    builder.RegisterType<ResourceManagerStringLocalizerFactory>()
                        .As<IStringLocalizerFactory>()
                        .SingleInstance();
                    builder.RegisterGeneric(typeof(StringLocalizer<>))
                        .As(typeof(IStringLocalizer<>))
                        .InstancePerLifetimeScope();
                    builder.RegisterModule(new LocalizationModule());
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