using System;
using Autofac;
using Autofac.Extras.Moq;
using Newbe.Claptrap.Preview.Abstractions;
using Newbe.Claptrap.Preview.Impl.Modules;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public static class AutoMockHelper
    {
        public static AutoMock Create(
            ITestOutputHelper testOutputHelper = null,
            bool localizationModule = true,
            DateTime? nowTime = null,
            bool verifyAll = true,
            Action<ContainerBuilder> builderAction = null)
        {
            var action = builderAction ?? (builder => { });
            var mocker = AutoMock.GetStrict(builder =>
            {
                action(builder);
                if (testOutputHelper != null)
                {
                    builder.AddLogging(testOutputHelper);
                }

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