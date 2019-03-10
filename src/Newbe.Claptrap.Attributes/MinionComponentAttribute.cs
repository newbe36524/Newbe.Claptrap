using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MinionComponentAttribute : Attribute
    {
        public string MinionCatalog { get; }
        public string Catalog { get; }

        public MinionComponentAttribute(string minionCatalog, string catalog)
        {
            MinionCatalog = minionCatalog;
            Catalog = catalog;
        }
    }
}