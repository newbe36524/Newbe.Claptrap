﻿using System.Threading.Tasks;
using Dapr.Actors.Runtime;

namespace Newbe.Claptrap.Dapr
{
    public abstract class ClaptrapBoxActor<TStateData> : Actor,
        IClaptrapBoxActor<TStateData>
        where TStateData : IStateData
    {
        private readonly ActorHost _actorService;

        protected ClaptrapBoxActor(
            ActorHost actorService,
            IClaptrapActorCommonService claptrapActorCommonService) : base(actorService)
        {
            _actorService = actorService;
            ClaptrapActorCommonService = claptrapActorCommonService;
        }

        public IClaptrapActorCommonService ClaptrapActorCommonService { get; }

        public IClaptrap Claptrap =>
            ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!;

        public TStateData StateData =>
            (TStateData) ClaptrapActorCommonService.ClaptrapAccessor.Claptrap!.State.Data;

        protected override async Task OnActivateAsync()
        {
            var actorTypeCode = ClaptrapActorCommonService.ClaptrapTypeCodeFactory.GetClaptrapTypeCode(this);
            var actorIdentity = new ClaptrapIdentity(_actorService.Id.GetId(), actorTypeCode);
            var claptrap = ClaptrapActorCommonService.ClaptrapFactory.Create(actorIdentity);
            await claptrap.ActivateAsync();
            ClaptrapActorCommonService.ClaptrapAccessor.Claptrap = claptrap;
        }

        protected override Task OnDeactivateAsync()
        {
            return Claptrap.DeactivateAsync();
        }
    }
}