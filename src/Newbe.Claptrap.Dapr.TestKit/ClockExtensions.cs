using System;
using Autofac.Extras.Moq;

namespace Newbe.Claptrap.Dapr.TestKit
{
    public static class ClockExtensions
    {
        public static DateTimeOffset FreezeNow(this AutoMock mocker)
        {
            var now = DateTime.Now;
            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(now);
            return now;
        }
    }
}