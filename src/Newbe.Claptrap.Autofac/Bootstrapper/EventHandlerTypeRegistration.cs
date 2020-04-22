using System;

namespace Newbe.Claptrap.Autofac
{
    public class EventHandlerTypeRegistration
    {
        public string EventTypeCode { get; set; }
        public string ActorTypeCode { get; set; }
        public Type EventHandlerType { get; set; }
    }
}