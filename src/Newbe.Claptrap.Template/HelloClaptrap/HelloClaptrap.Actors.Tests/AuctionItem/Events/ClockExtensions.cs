using System;
using Autofac.Extras.Moq;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.Tests.AuctionItem.Events
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