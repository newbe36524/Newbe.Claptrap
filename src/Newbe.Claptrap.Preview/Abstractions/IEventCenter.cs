using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IEventCenter
    {
        Task SendToMinionsAsync(IClaptrapIdentity identity, IEvent @event);
    }
}