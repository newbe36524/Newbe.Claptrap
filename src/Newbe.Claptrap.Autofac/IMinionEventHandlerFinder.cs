using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public interface IMinionEventHandlerFinder
    {
        IEnumerable<MinionEventHandlerRegistration> FindAll(Type[] allTypes);
    }
}