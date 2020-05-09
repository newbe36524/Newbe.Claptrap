using System;

namespace Newbe.Claptrap
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateRecoveryOptionsAttribute:Attribute
    {
        public StateRecoveryStrategy StateRecoveryStrategy { get; set; }
    }
}