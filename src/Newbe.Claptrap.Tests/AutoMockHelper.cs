using System;
using System.Globalization;
using Autofac;
using Autofac.Extras.Moq;
using Newbe.Claptrap.Localization.Modules;

namespace Newbe.Claptrap.Tests
{
    public static class AutoMockHelper
    {
        public static AutoMock Create(
            bool localizationModule = true,
            DateTime? nowTime = null,
            bool verifyAll = true,
            Action<ContainerBuilder> builderAction = null,
            CultureInfo cultureInfo = null)
        {
            var action = builderAction ?? (builder => { });
            var mocker = AutoMock.GetStrict(builder =>
            {
                action(builder);
                builder.RegisterModule<TestLoggingModule>();

                if (localizationModule)
                {
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