using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Models.AuctionItem.Events;
using HelloClaptrap.Models.AuctionItemUserCountMinion;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.AuctionItemUserCountMinion.Events
{
    public class NewBidderEventHandler
        : NormalEventHandler<AuctionItemUserCountState, NewBidderEvent>
    {
        private readonly IClock _clock;

        public NewBidderEventHandler(
            IClock clock)
        {
            _clock = clock;
        }

        public override ValueTask HandleEvent(AuctionItemUserCountState stateData, NewBidderEvent eventData,
            IEventContext eventContext)
        {
            var dic = stateData.UserBiddingCount ?? new Dictionary<int, int>();
            if (!dic.TryGetValue(eventData.UserId, out var nowValue))
            {
                nowValue = 0;
            }

            nowValue++;
            dic[eventData.UserId] = nowValue;
            stateData.UserBiddingCount = dic;
            return ValueTask.CompletedTask;
        }
    }
}