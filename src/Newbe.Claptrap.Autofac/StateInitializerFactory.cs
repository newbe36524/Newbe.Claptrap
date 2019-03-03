using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public class StateInitializerFactory : IStateInitializerFactory
    {
        private readonly Factory _factory;

        public delegate EventSourcingStateInitializer Factory(
            EventSourcingStateBuilderOptions options);

        public StateInitializerFactory(
            Factory factory)
        {
            _factory = factory;
        }

        public IStateInitializer Create(IActorIdentity actorIdentity)
        {
            var re = _factory(new EventSourcingStateBuilderOptions
            {
                RestoreEventVersionCountPerTime = 5000
            });
            return re;
        }
    }
}