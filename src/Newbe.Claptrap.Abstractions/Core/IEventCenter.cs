using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IEventCenter
    {
        Task SendToMinionsAsync(IClaptrapIdentity identity, IEvent @event);
    }
}