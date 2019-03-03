using System;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ClaptrapAttribute : ActorAttribute
    {
        public string Catalog { get; }
        public Type StateDataType { get; }

        public ClaptrapAttribute(string catalog, Type stateDataType)
            : base(ActorType.Claptrap)
        {
            Catalog = catalog;
            StateDataType = stateDataType;
        }
    }
}