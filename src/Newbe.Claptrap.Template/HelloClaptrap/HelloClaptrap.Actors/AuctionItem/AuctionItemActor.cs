using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using HelloClaptrap.Actors.AuctionItem.Events;
using HelloClaptrap.IActor;
using HelloClaptrap.Models;
using HelloClaptrap.Models.AuctionItem;
using HelloClaptrap.Models.AuctionItem.Events;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace HelloClaptrap.Actors.AuctionItem
{
    [Actor(TypeName = ClaptrapCodes.AuctionItemActor)]
    [ClaptrapStateInitialFactoryHandler(typeof(AuctionItemActorInitialStateDataFactory))]
    [ClaptrapStateStore(null, typeof(DecoratedStateLoaderFactory<AuctionItemActorStateLoader>))]
    [ClaptrapEventHandler(typeof(NewBidderEventHandler), ClaptrapCodes.NewBidderEvent)]
    public class AuctionItemActor : ClaptrapBoxActor<AuctionItemState>, IAuctionItemActor
    {
        private readonly IClock _clock;

        public AuctionItemActor(
            ActorHost actorHost,
            IClaptrapActorCommonService claptrapActorCommonService,
            IClock clock) : base(actorHost, claptrapActorCommonService)
        {
            _clock = clock;
        }

        public Task<AuctionItemStatus> GetStatusAsync()
        {
            return Task.FromResult(GetStatusCore());
        }

        private AuctionItemStatus GetStatusCore()
        {
            var now = _clock.UtcNow;
            if (now < StateData.StartTime)
            {
                return AuctionItemStatus.Planned;
            }

            if (now > StateData.StartTime && now < StateData.EndTime)
            {
                return AuctionItemStatus.OnSell;
            }

            return StateData.BiddingRecords?.Any() == true ? AuctionItemStatus.Sold : AuctionItemStatus.UnSold;
        }

        public Task<TryBiddingResult> TryBidding(TryBiddingInput input)
        {
            var status = GetStatusCore();

            if (status != AuctionItemStatus.OnSell)
            {
                return Task.FromResult(CreateResult(false));
            }

            if (input.Price <= GetTopPrice())
            {
                return Task.FromResult(CreateResult(false));
            }

            return HandleCoreAsync();

            async Task<TryBiddingResult> HandleCoreAsync()
            {
                var dataEvent = this.CreateEvent(new NewBidderEvent
                {
                    Price = input.Price,
                    UserId = input.UserId
                });
                await Claptrap.HandleEventAsync(dataEvent);
                return CreateResult(true);
            }

            TryBiddingResult CreateResult(bool success)
            {
                return new()
                {
                    Success = success,
                    NowPrice = GetTopPrice(),
                    UserId = input.UserId,
                    AuctionItemStatus = status
                };
            }
        }

        private decimal GetTopPrice()
        {
            return StateData.BiddingRecords?.Any() == true
                ? StateData.BiddingRecords.First().Key
                : StateData.BasePrice;
        }

        public Task<AuctionItemState> GetStateAsync()
        {
            return Task.FromResult(StateData);
        }

        public Task<decimal> GetTopPriceAsync()
        {
            return Task.FromResult(GetTopPrice());
        }
    }
}