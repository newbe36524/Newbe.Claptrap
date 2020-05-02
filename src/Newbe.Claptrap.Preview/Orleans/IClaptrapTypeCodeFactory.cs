using Newbe.Claptrap.Preview.Abstractions.Box;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapTypeCodeFactory
    {
        string GetClaptrapTypeCode(IClaptrapBox claptrapBox);
        string FindEventTypeCode<TEventDataType>(IClaptrapBox claptrapBox, TEventDataType eventDataType);
    }
}