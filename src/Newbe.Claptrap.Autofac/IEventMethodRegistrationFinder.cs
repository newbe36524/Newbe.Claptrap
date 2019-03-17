using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public interface IEventMethodRegistrationFinder
    {
        IEnumerable<EventMethodRegistration> FindAll(Type[] types);
    }
}