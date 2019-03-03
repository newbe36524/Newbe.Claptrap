using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.AddBalanceImpl;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.LockImpl;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.TransferImpl;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.Demo.Models;
using Newbe.Claptrap.Demo.Models.EventData;
using Orleans;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps
{
    public partial class Account
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

        public IActor Actor { get; private set; }
        public AccountStateData ActorState => (AccountStateData) Actor.State.Data;

        public IAddBalanceMethod AddBalanceMethod =>
            (IAddBalanceMethod) ServiceProvider.GetService(typeof(IAddBalanceMethod));

        public async Task AddBalance(decimal amount)
        {
            var result = await AddBalanceMethod.Invoke((AccountStateData) Actor.State.Data, amount);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, nameof(BalanceChangeEventData), result.EventData,
                    result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }

        public ITransferMethod TransferMethod =>
            (ITransferMethod) ServiceProvider.GetService(typeof(ITransferMethod));

        public async Task<TransferResult> Transfer(decimal amount)
        {
            var result = await TransferMethod.Invoke((AccountStateData) Actor.State.Data, amount);
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