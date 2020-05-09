using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateSavingOptionsAttribute:Attribute
    {
        public TimeSpan? SavingWindowTime { get; set; }
        public int? SavingWindowVersionLimit { get; set; }

        /// <summary>
        /// save state when claptrap deactivated or not
        /// </summary>
        public bool SaveWhenDeactivateAsync { get; set; }
    }
}