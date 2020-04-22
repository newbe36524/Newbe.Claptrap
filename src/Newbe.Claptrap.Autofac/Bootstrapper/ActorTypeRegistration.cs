using System;

namespace Newbe.Claptrap.Autofac
{
    public class ActorTypeRegistration
    {
        public string ActorTypeCode { get; set; }
        public Type StateInitialFactoryHandlerType { get; set; }
        public Type ActorStateDataType { get; set; }
    }
}