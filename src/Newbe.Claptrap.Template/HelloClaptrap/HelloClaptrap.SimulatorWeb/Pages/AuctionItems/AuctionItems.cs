using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using HelloClaptrap.Models.AuctionItem;
using HelloClaptrap.SimulatorWeb.Services;
using Microsoft.AspNetCore.Components;

namespace HelloClaptrap.SimulatorWeb.Pages.AuctionItems
{
    public partial class AuctionItems
    {
        [Inject] public IAuctionApi AuctionApi { get; set; }

        public AuctionItemsModel Model { get; set; } = new();

        public async Task StartAsync()
        {
            Task.Run(async () =>
            {
                AppendLog("Start Auction");
                var ids = Enumerable.Range(0, 100);
                foreach (var id in ids)
                {
                    var status = await AuctionApi.GetStatusAsync(id);
                    if (status.Status == AuctionItemStatus.OnSell)
                    {
                        Model.OnSellingItemId = id;
                        break;
                    }
                }

                AppendLog($"ItemId : {Model.OnSellingItemId} is OnSell.");

                var rd = new Random();

                while (true)
                {
                    var topPriceModel = await AuctionApi.GetTopPriceAsync(Model.OnSellingItemId);
                    var userId = rd.Next(0, 10);
                    var diff = rd.NextDouble() > 0.5 ? 2.1M : -1.1M;
                    var biddingPrice = topPriceModel.TopPrice + diff;
                    var tryBiddingResult = await AuctionApi.TryBiddingResultAsync(new TryBiddingWebApiInput
                    {
                        Price = biddingPrice,
                        ItemId = Model.OnSellingItemId,
                        UserId = userId
                    });
                    var nowPrice = tryBiddingResult.NowPrice;
                    AppendLog($"Try bidding with {biddingPrice}");
                    if (tryBiddingResult.Success)
                    {
                        Model.Records.Push(new BiddingRecord
                        {
                            Price = biddingPrice,
                            UserId = userId,
                            BiddingTime = DateTimeOffset.UtcNow
                        });
                        AppendLog($"Try bidding success by UserId {userId}, new price: {nowPrice}");
                    }
                    else
                    {
                        AppendLog($"Try bidding failed by UserId {userId}, now price: {nowPrice}", AlertType.Error);
                    }

                    var biddingCount = await AuctionApi.GetBiddingCount(Model.OnSellingItemId);
                    Model.UserBiddingCount = biddingCount;
                    StateHasChanged();

                    await Task.Delay(TimeSpan.FromSeconds(Model.SleepInSeconds));
                }
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    AppendLog($"Error: {t.Exception}", AlertType.Error);
                }
            });
        }

        private void AppendLog(string log, string level = AlertType.Info)
        {
            var item = new LogItem
            {
                Message = log,
                Level = level
            };
            Model.Logs.Insert(0, item);
            if (Model.Records.Count > 10)
            {
                Model.Logs.RemoveAt(Model.Logs.Count - 1);
            }

            StateHasChanged();
        }
    }

    public class AuctionItemsModel
    {
        public double SleepInSeconds { get; set; } = 1;
        public int OnSellingItemId { get; set; }
        public Dictionary<int, int> UserBiddingCount { get; set; } = new();
        public Stack<BiddingRecord> Records { get; set; } = new();
        public List<LogItem> Logs { get; set; } = new();
    }

    public record LogItem
    {
        public string Level { get; set; }
        public string Message { get; set; }
    }
}