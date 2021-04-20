using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Models;
using HelloClaptrap.Models.AuctionItemUserCountMinion;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr.Core;

namespace HelloClaptrap.IActor
{
    [ClaptrapMinion(ClaptrapCodes.AuctionItemActor)]
    [ClaptrapState(typeof(AuctionItemUserCountState), ClaptrapCodes.AuctionItemUserCountMinionActor)]
    public interface IAuctionItemUserCountMinionActor : IClaptrapMinionActor
    {
        Task<Dictionary<int, int>> GetUserBiddingCountAsync();
    }
}