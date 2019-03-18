using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.Demo.Interfaces.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Demo.Models.DomainService.TransferAccountBalance;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.DomainService.TransferAccountBalance.Claptrap
{
    [ClaptrapComponent("TransferAccountBalance")]
    public partial class TransferAccountBalance : Grain, ITransferAccountBalance
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var kind = new ClaptrapKind(ActorType.Claptrap, "TransferAccountBalance");
            var identity = new GrainActorIdentity(kind, this.GetPrimaryKeyString());
            var factory = (IActorFactory) ServiceProvider.GetService(typeof(IActorFactory));
            Actor = factory.Create(identity);
            await Actor.ActivateAsync();
        }

        public async override Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            await Actor.DeactivateAsync();
        }

        public IActor Actor { get; private set; }

        public TransferAccountBalanceStateData ActorState => (TransferAccountBalanceStateData) Actor.State.Data;

 
        public async Task<TransferResult> Transfer(string fromId, string toId, decimal balance)
        {
            DeactivateOnIdle();
            if (ActorState.Finished)
            {
                return new TransferResult
                {
                    Error = string.Empty
                };
            }

            var uid = Actor.State.Identity.Id;
            var fromAccount = GrainFactory.GetGrain<IAccount>(fromId);
            var transferResult = await fromAccount.TransferOut(balance, uid);
            if (string.IsNullOrEmpty(transferResult.Error))
            {
                var toAccount = GrainFactory.GetGrain<IAccount>(toId);
                await toAccount.TransferIn(balance, uid);
                var @event = new DataEvent(Actor.State.Identity, nameof(TransferAccountBalanceFinishedEventData),
                    new TransferAccountBalanceFinishedEventData(),
                    new EventUid(uid));
                await Actor.HandleEvent(@event);
            }

            return transferResult;
        }
    }
}