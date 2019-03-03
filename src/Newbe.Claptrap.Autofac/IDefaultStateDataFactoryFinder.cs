using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Autofac
{
    public interface IDefaultStateDataFactoryFinder
    {
        IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types);
    }
}