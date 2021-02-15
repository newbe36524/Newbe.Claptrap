using System;
using System.Threading.Tasks;
using FluentAssertions;
using HelloClaptrap.Actors.AuctionItem;
using HelloClaptrap.IActor;
using HelloClaptrap.Models.AuctionItem;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr.TestKit;
using NUnit.Framework;

namespace HelloClaptrap.Actors.Tests.AuctionItem
{
    public class AuctionItemActorTest
    {
        [Theory]
        [TestCase(19, AuctionItemStatus.Planned)]
        [TestCase(20, AuctionItemStatus.OnSell)]
        [TestCase(21, AuctionItemStatus.UnSold)]
        public async Task StatusWithNoBidder(int hour, AuctionItemStatus expectedStatus)
        {
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00")
            };
            var claptrapDesign = ActorTestHelper.GetDesign(typeof(AuctionItemActor));
            using var mocker = claptrapDesign.CreateAutoMock("1", state);

            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, hour, 0, 0));

            var auctionItemActor = mocker.Create<AuctionItemActor>();
            // act
            var status = await auctionItemActor.GetStatusAsync();

            // assert
            status.Should().Be(expectedStatus);
        }

        [Test]
        public async Task Sold()
        {
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00"),
            };
            state.InitBiddingRecords();
            state.BiddingRecords[1] = new BiddingRecord
            {
                Price = 1
            };
            var design = ActorTestHelper.GetDesign(typeof(AuctionItemActor));
            using var mocker = design.CreateAutoMock("11", state);
            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, 21, 0, 0));

            var auctionItemActor = mocker.Create<AuctionItemActor>();
            // act
            var status = await auctionItemActor.GetStatusAsync();

            // assert
            status.Should().Be(AuctionItemStatus.Sold);
        }


        [Theory]
        [TestCase(10, null, 5, false)]
        [TestCase(10, null, 10, false)]
        [TestCase(10, null, 11, true)]
        [TestCase(10, 10, 5, false)]
        [TestCase(10, 10, 11, true)]
        [TestCase(10, 11, 11, false)]
        public async Task TryBidding(decimal basePrice, decimal? topPrice, decimal biddingPrice, bool success)
        {
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00"),
                BasePrice = basePrice,
            };
            var design = ActorTestHelper.GetDesign(typeof(AuctionItemActor));
            using var mocker = design.CreateAutoMock("1", state);
            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, 20, 0, 0));

            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, 20, 0, 0));

            if (topPrice != null)
            {
                state.InitBiddingRecords();
                state.BiddingRecords[topPrice.Value] = new BiddingRecord
                {
                    Price = topPrice.Value,
                    UserId = 11
                };
            }

            var auctionItemActor = mocker.Create<AuctionItemActor>();
            // act
            var input = new TryBiddingInput
            {
                UserId = 777,
                Price = biddingPrice
            };
            var result = await auctionItemActor.TryBidding(input);

            // assert
            var nowPrice = Math.Max(biddingPrice, basePrice);
            if (topPrice.HasValue)
            {
                nowPrice = Math.Max(nowPrice, topPrice.Value);
            }

            var expectedResult = new TryBiddingResult
            {
                Success = success,
                NowPrice = nowPrice,
                UserId = input.UserId,
                AuctionItemStatus = AuctionItemStatus.OnSell
            };
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}