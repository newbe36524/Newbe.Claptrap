using System;

namespace Newbe.Claptrap.Preview.Abstractions.Design
{
    public interface IClaptrapEventHandlerDesign
    {
        public string EventTypeCode { get; }
        public Type EventDataType { get; }
        public Type EventHandlerType { get; }
    }
}