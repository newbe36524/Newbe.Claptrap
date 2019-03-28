using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using System;
using System.Threading.Tasks;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Minion
{
    [MinionComponent("TestClaptrap", "Database")]
    public partial class TestClaptrap : Grain, ITestClaptrap
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var actorFactory = (IActorFactory)ServiceProvider.GetService(typeof(IActorFactory));
            var identity =
                new GrainActorIdentity(new MinionKind(ActorType.Minion, "TestClaptrap", "Database"),
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
        public Task TestTaskEvent(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
        public Task TestTaskEvent2(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
        public Task HandleOtherEvent(IEvent @event)
        {
            return Actor.HandleEvent(@event);
        }
    }
}