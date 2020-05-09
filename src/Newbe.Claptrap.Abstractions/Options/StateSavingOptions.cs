using System;

namespace Newbe.Claptrap.Options
{
    public class StateSavingOptions
    {
        public TimeSpan? SavingWindowTime { get; set; }
        public int? SavingWindowVersionLimit { get; set; }

        /// <summary>
        /// save state when claptrap deactivated or not
        /// </summary>
        public bool SaveWhenDeactivateAsync { get; set; }
    }
}