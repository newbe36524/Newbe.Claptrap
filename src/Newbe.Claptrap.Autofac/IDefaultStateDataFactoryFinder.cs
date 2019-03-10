using System;
using System.Collections.Generic;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public interface IDefaultStateDataFactoryFinder
    {
        /// <summary>
        /// to find <see cref="IDefaultStateDataFactory"/> in types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IEnumerable<DefaultStateDataFactoryRegistration> FindAll(Type[] types);
    }
}