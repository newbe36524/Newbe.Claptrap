using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.Lock;
using Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.TransferIn;
using Newbe.Claptrap.Demo.Domain.Account.Claptrap._20EventMethods.TransferOut;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.Domain.Account;
using Newbe.Claptrap.Orleans;
using Orleans;

namespace Newbe.Claptrap.Demo.Domain.Account.Claptrap
{
    [ClaptrapComponent("Account")]
    public partial class Account : Grain, IAccount
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var kind = new ClaptrapKind(ActorType.Claptrap, "Account");
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
        public AccountStateData ActorState => (AccountStateData) Actor.State.Data;

        public async Task TransferIn(decimal amount, string uid)
        {
            var method = (ITransferInMethod) ServiceProvider.GetService(typeof(ITransferInMethod));
            var result = await method.Invoke((AccountStateData) Actor.State.Data, amount, uid);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, nameof(BalanceChangeEventData), result.EventData,result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }

        public ITransferOutMethod TransferOutMethod =>
            (ITransferOutMethod) ServiceProvider.GetService(typeof(ITransferOutMethod));

        public async Task<TransferResult> TransferOut(decimal amount,string uid)
        {
            var result = await TransferOutMethod.Invoke((AccountStateData) Actor.State.Data, amount, uid);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, nameof(BalanceChangeEventData), result.EventData,
                    result.EventUid);
                await Actor.HandleEvent(@event);
            }

            return result.MethodReturn;
        }

        public ILockMethod LockMethod =>
            (ILockMethod) ServiceProvider.GetService(typeof(ILockMethod));

        public async Task Lock()
        {
            var result = await LockMethod.Invoke((AccountStateData) Actor.State.Data);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, nameof(LockEventData), result.EventData,
                    result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
    }
}