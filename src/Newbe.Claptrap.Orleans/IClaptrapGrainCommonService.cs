namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IClaptrapFactory ClaptrapFactory { get; }
        IClaptrapAccessor ClaptrapAccessor { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
    }
}