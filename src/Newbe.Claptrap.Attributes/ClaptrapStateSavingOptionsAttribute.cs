using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the state saving options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateSavingOptionsAttribute : Attribute
    {
        public TimeSpan? SavingWindowTime { get; set; }
        public int? SavingWindowVersionLimit { get; set; }

        /// <summary>
        /// save state when claptrap deactivated or not
        /// </summary>
        public bool SaveWhenDeactivateAsync { get; set; }
    }
}