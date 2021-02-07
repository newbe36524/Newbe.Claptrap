using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using FluentAssertions;
using HelloClaptrap.Actors.AuctionItem;
using HelloClaptrap.Actors.AuctionItem.Events;
using HelloClaptrap.IActor;
using HelloClaptrap.Models.AuctionItem;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr;
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
            using var mocker = AutoMock.GetLoose(builder =>
            {
                builder.RegisterInstance(new ActorHost(ActorTypeInformation.Get(typeof(AuctionItemActor)),
                    new ActorId("11"),
                    default,
                    new NullLoggerFactory(),
                    default));
            });

            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, hour, 0, 0));
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00")
            };
            mocker.Mock<IClaptrapActorCommonService>()
                .Setup(x => x.ClaptrapAccessor.Claptrap.State.Data)
                .Returns(state);

            var auctionItemActor = mocker.Create<AuctionItemActor>();
            // act
            var status = await auctionItemActor.GetStatusAsync();

            // assert
            status.Should().Be(expectedStatus);
        }

        [Test]
        public async Task Sold()
        {
            using var mocker = AutoMock.GetLoose(builder =>
            {
                builder.RegisterInstance(new ActorHost(ActorTypeInformation.Get(typeof(AuctionItemActor)),
                    new ActorId("11"),
                    default,
                    new NullLoggerFactory(),
                    default));
            });

            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, 21, 0, 0));
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00"),
                BiddingRecords = new SortedDictionary<decimal, BiddingRecord>
                {
                    {
                        1, new BiddingRecord
                        {
                            Price = 1
                        }
                    }
                }
            };
            mocker.Mock<IClaptrapActorCommonService>()
                .Setup(x => x.ClaptrapAccessor.Claptrap.State.Data)
                .Returns(state);

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
            using var mocker = AutoMock.GetLoose(builder =>
            {
                builder.RegisterInstance(new ActorHost(ActorTypeInformation.Get(typeof(AuctionItemActor)),
                    new ActorId("11"),
                    default,
                    new NullLoggerFactory(),
                    default));
            });

            mocker.Mock<IClaptrapActorCommonService>()
                .Setup(x => x.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(It.IsAny<IClaptrapBox>()))
                .Returns("code");

            mocker.Mock<IClock>()
                .Setup(x => x.UtcNow)
                .Returns(new DateTime(2020, 01, 01, 20, 0, 0));
            var state = new AuctionItemState
            {
                StartTime = DateTimeOffset.Parse("2020-01-01 19:30:00"),
                EndTime = DateTimeOffset.Parse("2020-01-01 20:30:00"),
                BasePrice = basePrice,
            };
            if (topPrice != null)
            {
                state.BiddingRecords = new SortedDictionary<decimal, BiddingRecord>
                {
                    {
                        topPrice.Value,
                        new BiddingRecord
                        {
                            Price = topPrice.Value,
                            UserId = 11
                        }
                    }
                };
            }

            mocker.Mock<IClaptrapActorCommonService>()
                .Setup(x => x.ClaptrapAccessor.Claptrap.State.Data)
                .Returns(state);

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

            // var expectedResult = new TryBiddingResult
            // {
            //     Success = success,
            //     NowPrice = nowPrice,
            //     UserId = input.UserId,
            //     AuctionItemStatus = AuctionItemStatus.OnSell
            // };
            // result.Should().BeEquivalentTo(expectedResult);
            mocker.Mock<IClaptrapActorCommonService>()
                .Verify(x => x.ClaptrapAccessor.Claptrap.HandleEventAsync(It.IsAny<IEvent>()),
                    Times.Exactly(success ? 1 : 0));
        }
    }
}