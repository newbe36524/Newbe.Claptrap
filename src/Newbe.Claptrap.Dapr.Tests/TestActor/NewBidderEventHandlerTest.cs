using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Newbe.Claptrap.Dapr.TestKit;
using Newbe.Claptrap.Dapr.Tests.TestActor.Core;
using Newbe.Claptrap.TestSuit;
using NUnit.Framework;

namespace Newbe.Claptrap.Dapr.Tests.TestActor
{
    public class NewBidderEventHandlerTest
    {
        [Test]
        public async Task AddFirstBiddingRecord()
        {
            using var mocker = AutoMock.GetStrict();

            var now = mocker.FreezeNow();
            await using var handler = mocker.Create<NewBidderEventHandler>();
            var state = new AuctionItemState();
            var eventData = new NewBidderEvent
            {
                Price = 2,
                UserId = 1
            };

            // act
            await handler.HandleEvent(state, eventData, default);

            // assert
            var (key, value) = state.BiddingRecords.Single();
            key.Should().Be(eventData.Price);
            value.UserId.Should().Be(eventData.UserId);
            value.Price.Should().Be(eventData.Price);
            value.BiddingTime.Should().Be(now);
        }

        [Test]
        public async Task TheTopIsTheHighestPrice()
        {
            using var mocker = AutoMock.GetStrict();

            var now = mocker.FreezeNow();
            await using var handler = mocker.Create<NewBidderEventHandler>();
            var state = new AuctionItemState();

            var topPrice = 9999;

            // act
            await handler.HandleEvent(state, new NewBidderEvent
            {
                Price = 2,
                UserId = 1
            }, default);
            await handler.HandleEvent(state, new NewBidderEvent
            {
                Price = topPrice,
                UserId = 2
            }, default);

            // assert
            var (key, value) = state.BiddingRecords.First();
            key.Should().Be(topPrice);
        }
    }
}