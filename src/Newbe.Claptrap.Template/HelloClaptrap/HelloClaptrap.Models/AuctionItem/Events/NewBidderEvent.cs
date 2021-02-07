using Newbe.Claptrap;

namespace HelloClaptrap.Models.AuctionItem.Events
{
    public record NewBidderEvent : IEventData
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
    }
}