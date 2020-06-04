using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Add display name for claptrap or event or event handler.
    /// It could make it easier for developer to understand code on viewer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public ClaptrapDisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }
}