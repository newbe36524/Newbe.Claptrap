using System.Collections.Generic;

namespace Newbe.Claptrap.Preview
{
    public class ClaptrapRegistration
    {
        public IEnumerable<ActorTypeRegistration> ActorTypeRegistrations { get; set; }
        public IEnumerable<EventTypeHandlerRegistration> EventHandlerTypeRegistrations { get; set; }
        public IEnumerable<EventStoreRegistration> EventStoreRegistrations { get; set; }
        public IEnumerable<StateStoreRegistration> StateStoreRegistrations { get; set; }
    }
}