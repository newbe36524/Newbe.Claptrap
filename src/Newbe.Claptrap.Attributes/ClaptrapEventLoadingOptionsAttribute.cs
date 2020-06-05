using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the event loading options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapEventLoadingOptionsAttribute : Attribute
    {
        /// <summary>
        /// max event load in one batch
        /// </summary>
        public int LoadingCountInOneBatch { get; set; }
    }
}