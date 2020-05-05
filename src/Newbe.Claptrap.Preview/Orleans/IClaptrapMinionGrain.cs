using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapMinionGrain : IClaptrapGrain
    {
        Task MasterCall(IEvent @event);
    }
}