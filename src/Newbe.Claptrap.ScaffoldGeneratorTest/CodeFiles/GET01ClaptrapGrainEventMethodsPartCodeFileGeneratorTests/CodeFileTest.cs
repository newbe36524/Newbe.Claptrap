using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using System;
using System.Threading.Tasks;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestStateDataType;
namespace Claptrap
{
    [ClaptrapComponent("TestClaptrap")]
    public partial class TestClaptrap : Grain, ITestClaptrap
    {
        public override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
            var kind = new ClaptrapKind(ActorType.Claptrap, "TestClaptrap");
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
        public async Task TestTaskMethod(int a, string b, DateTime c)
        {
            var method = (ITestTaskMethod)ServiceProvider.GetService(typeof(ITestTaskMethod));
            var result = await method.Invoke((StateData)Actor.State.Data, a, b, c);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, "Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType", result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
        public async Task TestTaskMethod2()
        {
            var method = (ITestTaskMethod2)ServiceProvider.GetService(typeof(ITestTaskMethod2));
            var result = await method.Invoke((StateData)Actor.State.Data);
            if (result.EventRaising)
            {
                var @event = new DataEvent(Actor.State.Identity, "Newbe.Claptrap.ScaffoldGeneratorTest.TestEventDataType", result.EventData, result.EventUid);
                await Actor.HandleEvent(@event);
            }
        }
    }
}
