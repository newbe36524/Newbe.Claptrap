using System;

namespace Newbe.Claptrap.Design
{
    public class ClaptrapEventHandlerDesign : IClaptrapEventHandlerDesign
    {
        public string EventTypeCode { get; set; } = null!;
        public Type EventDataType { get; set; } = null!;
        public Type EventHandlerType { get; set; } = null!;
    }
}