using System;

namespace Newbe.Claptrap.Autofac
{
    public class MinionEventHandlerRegistration
    {
        public MinionEventHandlerRegistration(MinionEventHandlerRegistrationKey key, Type type)
        {
            Key = key;
            Type = type;
        }

        public MinionEventHandlerRegistrationKey Key { get; }
        public Type Type { get; }
    }
}