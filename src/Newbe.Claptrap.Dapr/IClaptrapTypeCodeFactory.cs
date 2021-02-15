using System;

namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapTypeCodeFactory
    {
        string GetClaptrapTypeCode(IClaptrapBox claptrapBox);
        string GetClaptrapTypeCode(Type interfaceType);
        string FindEventTypeCode<TEventDataType>(IClaptrapBox claptrapBox, TEventDataType eventDataType);
    }
}