using System;

namespace Newbe.Claptrap.Preview.Abstractions.Metadata
{
    public class ClaptrapEventHandlerDesign : IClaptrapEventHandlerDesign
    {
        public string EventTypeCode { get; set; } = null!;
        public Type EventDataType { get; set; } = null!;
        public Type EventHandlerType { get; set; } = null!;
    }
}