
namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapTypeCodeFactory
    {
        string GetClaptrapTypeCode(IClaptrapBox claptrapBox);
        string FindEventTypeCode<TEventDataType>(IClaptrapBox claptrapBox, TEventDataType eventDataType);
    }
}