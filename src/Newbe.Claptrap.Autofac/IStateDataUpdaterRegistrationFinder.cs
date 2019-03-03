using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public interface IStateDataUpdaterRegistrationFinder
    {
        IEnumerable<StateDataUpdaterRegistration> FindAll(Type[] types);
    }
}