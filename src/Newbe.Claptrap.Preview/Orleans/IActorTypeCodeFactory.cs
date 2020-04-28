namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IActorTypeCodeFactory
    {
        string GetActorTypeCode(IClaptrap claptrap);
    }
}