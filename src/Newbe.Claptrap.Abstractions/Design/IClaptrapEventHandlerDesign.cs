using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public interface IClaptrapEventHandlerDesign
    {
        string EventTypeCode { get; }
        Type EventDataType { get; }
        Type EventHandlerType { get; }
        IDictionary<string, object> ExtendInfos { get; }
    }
}