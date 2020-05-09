using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapEventLoadingOptionsAttribute:Attribute
    {
        /// <summary>
        /// max event load in one batch
        /// </summary>
        public int LoadingCountInOneBatch { get; set; }
    }
}