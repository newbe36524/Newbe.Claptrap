using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EventHandledNotifierContext : IEventHandledNotifierContext
    {
        public IEvent Event { get; set; }
        public IState CurrentState { get; set; }
        public IState EarlierState { get; set; }
    }
}