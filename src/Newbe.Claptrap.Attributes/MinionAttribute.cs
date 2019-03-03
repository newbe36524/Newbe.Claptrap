using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class MinionAttribute : ActorAttribute
    {
        public string MinionCatalog { get; }

        public string Catalog { get; }
        public Type StateDataType { get; }

        public MinionAttribute(string minionCatalog, string catalog, Type stateDataType)
            : base(ActorType.Minion)
        {
            MinionCatalog = minionCatalog;
            Catalog = catalog;
            StateDataType = stateDataType;
        }
    }
}