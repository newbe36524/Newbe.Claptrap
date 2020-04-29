using System;

namespace Newbe.Claptrap.Preview.Abstractions.Metadata
{
    public interface IClaptrapEventHandlerDesign
    {
        public string EventTypeCode { get; }
        public Type EventDataType { get; }
        public Type EventHandlerType { get; }
    }
}