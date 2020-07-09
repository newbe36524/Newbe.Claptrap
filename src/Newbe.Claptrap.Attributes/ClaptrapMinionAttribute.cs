using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> implementation class to specify the event store options
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ClaptrapMinionAttribute : Attribute
    {
        public string MasterTypeCode { get; }

        public ClaptrapMinionAttribute(string masterTypeCode)
        {
            MasterTypeCode = masterTypeCode;
        }
    }
}