using System;

namespace Newbe.Claptrap.Autofac
{
    public class StateDataUpdaterRegistration
    {
        public StateDataUpdaterRegistration(StateDataUpdaterRegistrationKey key, Type type)
        {
            Key = key;
            Type = type;
        }

        public StateDataUpdaterRegistrationKey Key { get; set; }
        public Type Type { get; set; }
    }
}