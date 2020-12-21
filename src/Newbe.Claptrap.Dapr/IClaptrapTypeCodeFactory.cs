namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapTypeCodeFactory
    {
        string GetClaptrapTypeCode(IClaptrapBox claptrapBox);
        string FindEventTypeCode<TEventDataType>(IClaptrapBox claptrapBox, TEventDataType eventDataType);
    }
}