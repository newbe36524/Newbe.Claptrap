
using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap
{
    [ClaptrapComponent("TestCatalog")]
    public partial class TestClaptrapGrain : Grain, Newbe.Claptrap.ScaffoldGeneratorTest.ITestClaptrapGrain
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var kind = new ClaptrapKind(ActorType.Claptrap, "TestCatalog");
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
        public async System.Threading.Tasks.Task TestTaskMethod()
        {
            var method = (ITestTaskMethod)ServiceProvider.GetService(typeof(ITestTaskMethod));
            var result = await method.Invoke((StateData)Actor.State.Data);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, nameof(Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType), result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
    }
}