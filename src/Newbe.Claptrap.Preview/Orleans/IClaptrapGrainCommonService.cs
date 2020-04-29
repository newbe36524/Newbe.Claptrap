using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapGrainCommonService
    {
        IClaptrapFactory ClaptrapFactory { get; }
        IClaptrapTypeCodeFactory ClaptrapTypeCodeFactory { get; }
    }
}