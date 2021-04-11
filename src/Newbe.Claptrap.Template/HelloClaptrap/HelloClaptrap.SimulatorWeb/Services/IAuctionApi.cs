using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.IActor;
using HelloClaptrap.Models.AuctionItem;
using Refit;

namespace HelloClaptrap.SimulatorWeb.Services
{
    public interface IAuctionApi
    {
        [Get("/api/AuctionItems/{itemId}/status")]
        Task<StatusModel> GetStatusAsync(int itemId);

        [Get("/api/AuctionItems/{itemId}/topPrice")]
        Task<TopPriceModel> GetTopPriceAsync(int itemId);

        [Get("/api/AuctionItems/{itemId}")]
        Task<StateModel> GetStateAsync(int itemId);

        [Get("/api/AuctionItems/{itemId}/biddingcount")]
        Task<Dictionary<int, int>> GetBiddingCount(int itemId);

        [Post("/api/AuctionItems")]
        Task<TryBiddingResult> TryBiddingResultAsync(TryBiddingWebApiInput input);
    }

    public record StatusModel
    {
        public AuctionItemStatus Status { get; set; }
    }

    public record StateModel
    {
        public AuctionItemState State { get; set; }
    }

    public record TopPriceModel
    {
        public decimal TopPrice { get; set; }
    }


    public record TryBiddingWebApiInput
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public int ItemId { get; set; }
    }
}