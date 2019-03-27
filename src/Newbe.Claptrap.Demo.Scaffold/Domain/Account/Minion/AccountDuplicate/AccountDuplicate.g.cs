using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using StateData = Newbe.Claptrap.Demo.Models.Domain.Account.AccountDuplicateStateData;
namespace Newbe.Claptrap.Demo.Scaffold.Domain.Account.Minion.AccountDuplicate
{
    [MinionComponent("Account", "AccountDuplicate")]
    public partial class AccountDuplicate : Grain, IAccountDuplicate
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var actorFactory = (IActorFactory)ServiceProvider.GetService(typeof(IActorFactory));
            var identity =
                new GrainActorIdentity(new MinionKind(ActorType.Minion, "Account", "AccountDuplicate"),
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
        public Task HandleBalance(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }

        public Task HandleOtherEvent(IEvent @event)
        {
            throw new System.NotImplementedException();
        }
    }
}
