using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Models.AuctionItem;
using HelloClaptrap.Models.AuctionItem.Events;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.AuctionItem.Events
{
    public class NewBidderEventHandler
        : NormalEventHandler<AuctionItemState, NewBidderEvent>
    {
        private readonly IClock _clock;

        public NewBidderEventHandler(
            IClock clock)
        {
            _clock = clock;
        }

        public override ValueTask HandleEvent(AuctionItemState stateData,
            NewBidderEvent eventData,
            IEventContext eventContext)
        {
            var records = stateData.BiddingRecords ??
                          new SortedDictionary<decimal, BiddingRecord>(
                              Comparer<decimal>.Create((x, y) => Comparer<decimal>.Default.Compare(y, x)));

            records.Add(eventData.Price, new BiddingRecord
            {
                Price = eventData.Price,
                BiddingTime = _clock.UtcNow,
                UserId = eventData.UserId
            });
            stateData.BiddingRecords = records;
            return ValueTask.CompletedTask;
        }
    }
}