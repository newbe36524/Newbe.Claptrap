using System;

namespace Newbe.Claptrap.Preview.Attributes
{
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