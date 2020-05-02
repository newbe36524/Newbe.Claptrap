using Newbe.Claptrap.Preview.Abstractions.Box;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IClaptrapBoxFactory BoxFactory { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
    }
}