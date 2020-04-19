using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Orleans;
using Orleans;
using StateData = Newbe.Claptrap.Core.NoneStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Minion.ActorFlow
{
    [MinionComponent("Account", "ActorFlow")]
    public partial class AccountActorFlowMinion : Grain, IAccountActorFlowMinion
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var actorFactory = (IActorFactory)ServiceProvider.GetService(typeof(IActorFactory));
            var identity =
                new GrainActorIdentity(new MinionKind(ActorType.Minion, "Account", "ActorFlow"),
            this.GetPrimaryKeyString());
            Actor = actorFactory.Create(identity);
            await Actor.ActivateAsync();
        }
        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            await Actor.DeactivateAsync();
        }
        public IActor Actor { get; private set; }
        public StateData ActorState => (StateData)Actor.State.Data;
        public Task BalanceChangeNotice(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
        public Task HandleOtherEvent(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
    }
}
