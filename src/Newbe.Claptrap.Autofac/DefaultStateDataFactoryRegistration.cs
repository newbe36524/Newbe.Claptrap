using System;

namespace Newbe.Claptrap.Autofac
{
    public class DefaultStateDataFactoryRegistration
    {
        public DefaultStateDataFactoryRegistration(Type type, DefaultStateDataFactoryRegistrationKey key)
        {
            Type = type;
            Key = key;
        }

        public Type Type { get; set; }
        public DefaultStateDataFactoryRegistrationKey Key { get; set; }
    }
}