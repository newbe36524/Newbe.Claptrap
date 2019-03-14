using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Interfaces.DomainService;
using Newbe.Claptrap.Demo.Models.EventData;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.DomainService.Claptrap
{
    [ClaptrapComponent("TransferAccountBalance")]
    public class TransferAccountBalance : Grain, ITransferAccountBalance
    {
        private readonly IGrainFactory _grainFactory;

        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();

            var kind = new ClaptrapKind(ActorType.Claptrap, "TransferAccountBalance");
            var identity = new GrainActorIdentity(kind, this.GetPrimaryKeyString());
            var factory = (IActorFactory) ServiceProvider.GetService(typeof(IActorFactory));
            Actor = factory.Create(identity);
            await Actor.ActivateAsync();
        }


        public IActor Actor { get; private set; }

        public TransferAccountBalanceStateData ActorState => (TransferAccountBalanceStateData) Actor.State.Data;

        public TransferAccountBalance(
            IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }

        public async Task<TransferResult> Transfer(string fromId, string toId, decimal balance)
        {
            if (ActorState.Finished)
            {
                return new TransferResult
                {
                    Error = string.Empty
                };
            }

            var uid = Actor.State.Identity.Id;
            var fromAccount = _grainFactory.GetGrain<IAccount>(fromId);
            var transferResult = await fromAccount.TransferOut(balance, uid);
            if (string.IsNullOrEmpty(transferResult.Error))
            {
                var toAccount = _grainFactory.GetGrain<IAccount>(toId);
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