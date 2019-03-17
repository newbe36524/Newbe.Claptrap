using System;
using System.Collections.Generic;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public interface IStateDataFactoryFinder
    {
        /// <summary>
        /// to find <see cref="IStateDataFactory"/> in types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types);
    }
}