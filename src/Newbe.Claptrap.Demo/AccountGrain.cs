using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo
{
    public class AccountGrain : Grain, IAccount
    {
        private readonly IActorFactory _actorFactory;

        public AccountGrain(IActorFactory actorFactory)
        {
            _actorFactory = actorFactory;
        }

        private IActor _actor;
        private GrainActorIdentity _grainActorIdentity = null!;

        public override async Task OnActivateAsync()
        {
            // TODO type code
            var typeCode = GetType().ToString();
            _grainActorIdentity = new GrainActorIdentity(this.GetPrimaryKeyString(), typeCode);
            _actor = _actorFactory.Create(_grainActorIdentity);
            await _actor.ActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            return _actor.DeactivateAsync();
        }

        public Task TransferIn(decimal amount, string uid)
        {
            var accountBalanceChangeEventData = new AccountBalanceChangeEventData
            {
                Diff = -amount
            };
            return _actor.HandleEvent(new DataEvent(_grainActorIdentity, "eventTy", accountBalanceChangeEventData,
                new EventUid(Guid.NewGuid().ToString())));
        }

        public Task<decimal> GetBalance()
        {
            var accountStateData = (AccountStateData) _actor.State.Data;
            var re = accountStateData.Balance;
            return Task.FromResult(re);
        }
    }
}