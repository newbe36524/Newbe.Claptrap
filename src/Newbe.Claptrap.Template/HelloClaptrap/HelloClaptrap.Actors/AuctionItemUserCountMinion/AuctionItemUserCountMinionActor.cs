﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using HelloClaptrap.Actors.AuctionItemUserCountMinion.Events;
using HelloClaptrap.IActor;
using HelloClaptrap.Models;
using HelloClaptrap.Models.AuctionItemUserCountMinion;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr;

namespace HelloClaptrap.Actors.AuctionItemUserCountMinion
{
    [Actor(TypeName = ClaptrapCodes.AuctionItemUserCountMinionActor)]
    [ClaptrapEventHandler(typeof(NewBidderEventHandler), ClaptrapCodes.NewBidderEvent)]
    public class AuctionItemUserCountMinionActor : ClaptrapBoxActor<AuctionItemUserCountState>,
        IAuctionItemUserCountMinionActor
    {
        public AuctionItemUserCountMinionActor(ActorHost actorHost,
            IClaptrapActorCommonService claptrapActorCommonService)
            : base(actorHost, claptrapActorCommonService)
        {
        }

        public Task<Dictionary<int, int>> GetUserBiddingCountAsync()
        {
            return Task.FromResult(StateData.UserBiddingCount);
        }
    }
}