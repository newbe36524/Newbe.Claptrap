using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountStateData;
namespace Claptrap
{
    [ClaptrapComponent("Account")]
    public partial class Account : Grain, IAccount
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var kind = new ClaptrapKind(ActorType.Claptrap, "Account");
            var identity = new GrainActorIdentity(kind, this.GetPrimaryKeyString());
            var factory = (IActorFactory)ServiceProvider.GetService(typeof(IActorFactory));
            Actor = factory.Create(identity);
            await Actor.ActivateAsync();
        }
        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            await Actor.DeactivateAsync();
        }
        public IActor Actor { get; private set; }
        public StateData ActorState => (StateData)Actor.State.Data;
        public async Task TransferIn(decimal amount, string uid)
        {
            var method = (ITransferInMethod)ServiceProvider.GetService(typeof(ITransferInMethod));
            var result = await method.Invoke((StateData)Actor.State.Data, amount, uid);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, "BalanceChangeEventData", result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
        public async Task<TransferResult> TransferOut(decimal amount, string uid)
        {
            var method = (ITransferOutMethod)ServiceProvider.GetService(typeof(ITransferOutMethod));
            var result = await method.Invoke((StateData)Actor.State.Data, amount, uid);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, "BalanceChangeEventData", result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
        public async Task Lock()
        {
            var method = (ILockMethod)ServiceProvider.GetService(typeof(ILockMethod));
            var result = await method.Invoke((StateData)Actor.State.Data);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, "LockEventData", result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
    }
}
