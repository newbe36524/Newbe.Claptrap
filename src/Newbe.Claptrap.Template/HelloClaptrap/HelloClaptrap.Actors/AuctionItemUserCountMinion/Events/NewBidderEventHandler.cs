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
        public override ValueTask HandleEvent(AuctionItemUserCountState stateData, NewBidderEvent eventData,
            IEventContext eventContext)
        {
            var dic = stateData.UserBiddingCount ?? new Dictionary<int, int>();
            var userId = eventData.UserId;
            if (!dic.TryGetValue(userId, out var nowValue))
            {
                nowValue = 0;
            }

            nowValue++;
            dic[userId] = nowValue;
            stateData.UserBiddingCount = dic;
            return ValueTask.CompletedTask;
        }
    }
}