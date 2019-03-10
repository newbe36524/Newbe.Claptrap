using System;

namespace Newbe.Claptrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClaptrapComponentAttribute : Attribute
    {
        public string Catalog { get; }

        public ClaptrapComponentAttribute(string catalog)
        {
            Catalog = catalog;
        }
    }
}