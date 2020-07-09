using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the state recovery options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateRecoveryOptionsAttribute : Attribute
    {
        public StateRecoveryStrategy StateRecoveryStrategy { get; set; }
    }
}