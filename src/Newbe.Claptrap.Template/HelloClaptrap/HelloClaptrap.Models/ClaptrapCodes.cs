namespace HelloClaptrap.Models
{
    public static class ClaptrapCodes
    {
        public const string AuctionItemActor = "auction_claptrap_newbe";
        private const string AuctionItemEventSuffix = "_e_" + AuctionItemActor;
        public const string NewBidderEvent = "newBidder" + AuctionItemEventSuffix;
    }
}