using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo
{
    [ClaptrapState(typeof(AccountStateData))]
    public class AccountGrain : Grain, IAccount
    {
        private readonly IActorFactory _actorFactory;
        private readonly IStateDataTypeRegister _stateDataTypeRegister;

        public AccountGrain(
            IActorFactory actorFactory,
            IStateDataTypeRegister stateDataTypeRegister)
        {
            _actorFactory = actorFactory;
            _stateDataTypeRegister = stateDataTypeRegister;
        }

        private IActor _actor;
        private GrainActorIdentity _grainActorIdentity = null!;

        public override async Task OnActivateAsync()
        {
            var actorTypeCode = _stateDataTypeRegister.FindActorTypeCode(typeof(AccountStateData));
            _grainActorIdentity = new GrainActorIdentity(this.GetPrimaryKeyString(), actorTypeCode);
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
            return _actor.HandleEvent(new DataEvent(_grainActorIdentity,
                typeof(AccountBalanceChangeEventData).FullName,
                accountBalanceChangeEventData,
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