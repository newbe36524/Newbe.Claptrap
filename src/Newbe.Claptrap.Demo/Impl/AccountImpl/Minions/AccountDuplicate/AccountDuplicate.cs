using System.Threading.Tasks;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Demo.Interfaces;
using Orleans;

namespace Newbe.Claptrap.Demo.Impl.AccountImpl.Minions.AccountDuplicate
{
    [MinionComponent("AccountDuplicate", "Account")]
    public class AccountDuplicate
        : Grain, IAccountDuplicate
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var actorFactory = (IActorFactory) ServiceProvider.GetService(typeof(IActorFactory));
            var identity =
                new GrainActorIdentity(new MinionKind(ActorType.Minion, "Account", "AccountDuplicate"),
                    this.GetPrimaryKeyString());
            Actor = actorFactory.Create(identity);
            await Actor.ActivateAsync();
        }

        public IActor Actor { get; private set; }

        public override async Task OnDeactivateAsync()
        {
            await base.OnDeactivateAsync();
            await Actor.DeactivateAsync();
        }

        public Task HandleBalance(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
    }
}