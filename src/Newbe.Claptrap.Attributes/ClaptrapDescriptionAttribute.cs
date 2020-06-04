using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Add description for claptrap or event or event handler.
    /// It could make it easier for developer to understand code on viewer.  
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ClaptrapDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}