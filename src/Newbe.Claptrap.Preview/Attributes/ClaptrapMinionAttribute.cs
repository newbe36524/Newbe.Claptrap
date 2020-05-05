using System;

namespace Newbe.Claptrap.Preview.Attributes
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class ClaptrapMinionAttribute : Attribute
    {
        public string MasterTypeCode { get; }
        public bool ActivateWhenMasterActivated { get; set; }

        public ClaptrapMinionAttribute(string masterTypeCode)
        {
            MasterTypeCode = masterTypeCode;
        }
    }
}