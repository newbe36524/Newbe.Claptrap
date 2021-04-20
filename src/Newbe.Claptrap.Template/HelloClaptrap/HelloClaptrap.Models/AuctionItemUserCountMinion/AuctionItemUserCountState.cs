using System.Collections.Generic;
using HelloClaptrap.Models.AuctionItem;
using Newbe.Claptrap;

namespace HelloClaptrap.Models.AuctionItemUserCountMinion
{
    public class AuctionItemUserCountState : IStateData
    {
        public Dictionary<int, int> UserBiddingCount { get; set; } = new();
    }
}