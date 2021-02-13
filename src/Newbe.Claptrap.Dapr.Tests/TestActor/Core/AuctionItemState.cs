using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Dapr.Tests.TestActor.Core
{
    public record AuctionItemState : IStateData
    {
        public SortedDictionary<decimal, BiddingRecord> BiddingRecords { get; set; }
        public decimal BasePrice { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public void InitBiddingRecords()
        {
            BiddingRecords =   new SortedDictionary<decimal, BiddingRecord>(
                Comparer<decimal>.Create((x, y) => Comparer<decimal>.Default.Compare(y, x)));
        }
    }

    public enum AuctionItemStatus
    {
        Planned,
        OnSell,
        Sold,
        UnSold
    }

    public record BiddingRecord
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset BiddingTime { get; set; }
    }
}