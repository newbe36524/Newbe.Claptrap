namespace Newbe.Claptrap.Orleans
{
    public interface IActorTypeCodeFactory
    {
        string GetActorTypeCode(IClaptrapGrain claptrapGrain);
    }
}