using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapMinionOptionsAttribute:Attribute
    {
        /// <summary>
        /// To activate minions at this claptrap start or not
        /// </summary>
        public bool ActivateMinionsAtStart { get; set; }
    }
}