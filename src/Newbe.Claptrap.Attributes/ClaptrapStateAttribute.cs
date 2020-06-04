using System;

namespace Newbe.Claptrap
{
    /// <summary>
    /// Mark on <see cref="IClaptrapBox"/> specify the state data class
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class ClaptrapStateAttribute : Attribute
    {
        public Type StateDataType { get; set; }
        public string ClaptrapTypeCode { get; set; }

        public ClaptrapStateAttribute(Type stateDataType, string claptrapTypeCode)
        {
            StateDataType = stateDataType;
            ClaptrapTypeCode = claptrapTypeCode;
        }
    }
}