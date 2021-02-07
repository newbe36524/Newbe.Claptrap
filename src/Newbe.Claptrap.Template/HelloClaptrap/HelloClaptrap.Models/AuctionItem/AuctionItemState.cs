using System;
using System.Collections.Generic;
using Newbe.Claptrap;

namespace HelloClaptrap.Models.AuctionItem
{
    public record AuctionItemState : IStateData
    {
        public SortedDictionary<decimal, BiddingRecord> BiddingRecords { get; set; }
        public decimal BasePrice { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
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