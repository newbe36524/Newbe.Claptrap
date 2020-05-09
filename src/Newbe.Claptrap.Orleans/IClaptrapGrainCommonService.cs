using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IClaptrapBoxFactory BoxFactory { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
    }
}