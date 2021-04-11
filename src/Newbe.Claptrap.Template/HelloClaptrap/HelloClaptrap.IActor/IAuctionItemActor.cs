using System.Threading.Tasks;
using HelloClaptrap.Models;
using HelloClaptrap.Models.AuctionItem;
using HelloClaptrap.Models.AuctionItem.Events;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr.Core;

namespace HelloClaptrap.IActor
{
    [ClaptrapState(typeof(AuctionItemState), ClaptrapCodes.AuctionItemActor)]
    [ClaptrapEvent(typeof(NewBidderEvent), ClaptrapCodes.NewBidderEvent)]
    public interface IAuctionItemActor : IClaptrapActor
    {
        Task<AuctionItemStatus> GetStatusAsync();
        Task<TryBiddingResult> TryBidding(TryBiddingInput input);
        Task<AuctionItemState> GetStateAsync();
        Task<decimal> GetTopPriceAsync();
    }

    public record TryBiddingResult
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public AuctionItemStatus AuctionItemStatus { get; set; }
        public decimal NowPrice { get; set; }
    }

    public record TryBiddingInput
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
    }
}